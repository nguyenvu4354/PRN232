using Microsoft.EntityFrameworkCore;
using ShoppingWeb.Models;
using ShoppingWeb.Services.Interface;
using ShoppingWeb.Services.ThirdParty;

namespace ShoppingWeb.Services
{
    public class AddressService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<AddressService> _logger;
        public AddressService(IServiceScopeFactory serviceScopeFactory, ILogger<AddressService> logger)
        {
            _scopeFactory = serviceScopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Serting getting address.");
                try
                {
                    await SyncAddressAsync(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while updating provinces.");
                }
                Task.Delay(TimeSpan.FromHours(48), stoppingToken).Wait();
            }
        }

        private async Task SyncAddressAsync(CancellationToken token)
        {
            using var scope = _scopeFactory.CreateScope();
            var ghn = scope.ServiceProvider.GetRequiredService<IGHN>();
            var alldistricts = new List<District>();
            var allwards = new List<Ward>();
            var provinces = await ghn.GetProvinces();
            _logger.LogInformation($"Got {provinces.Count} provinces");

            using var semaphore = new SemaphoreSlim(5);
            var provinceTask = provinces.Select(async province =>
            {
                await semaphore.WaitAsync(token);
                try
                {
                    var districts = await ghn.GetDistricts(province.Id);

                    using var districtSemaphore = new SemaphoreSlim(5);
                    var districtTasks = districts.Select(async district =>
                    {
                        await districtSemaphore.WaitAsync(token);
                        try
                        {
                            var ward = await ghn.GetWards(district.Id);
                            alldistricts.Add(district);
                            allwards.AddRange(ward);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"Error updating wards for district {district.Name}");
                        }
                        finally
                        {
                            districtSemaphore.Release();
                        }
                    }).ToList();
                    await Task.WhenAll(districtTasks);

                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error updating districts for province {province.Name}");
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToList();
            await Task.WhenAll(provinceTask);
            await SaveToDatabase(provinces, alldistricts, allwards);
            _logger.LogInformation("All addresses updated successfully.");
        }

        public async Task SaveToDatabase(List<Province> provinces, List<District> districts, List<Ward> wards)
        {
            using var scope = _scopeFactory.CreateScope();
            var _context = scope.ServiceProvider.GetRequiredService<ShoppingWebContext>();

            foreach (var province in provinces)
            {
                var existingProvince = await _context.Provinces.FirstOrDefaultAsync(p => p.Id == province.Id);
                if (existingProvince == null)
                {
                    _context.Provinces.Add(province);
                }
                else
                {
                    existingProvince.Name = province.Name;
                }
            }

            foreach (var district in districts)
            {
                var existingDistrict = await _context.Districts.FirstOrDefaultAsync(d => d.Id == district.Id);
                if (existingDistrict == null)
                {
                    _context.Districts.Add(district);
                }
                else
                {
                    existingDistrict.Name = district.Name;
                    existingDistrict.ProvinceId = district.ProvinceId;
                }
            }

            foreach (var ward in wards)
            {
                var existingWard = await _context.Wards.FirstOrDefaultAsync(w => w.Id == ward.Id);
                if (existingWard == null)
                {
                    _context.Wards.Add(ward);
                }
                else
                {
                    existingWard.Name = ward.Name;
                    existingWard.DistrictId = ward.DistrictId;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
