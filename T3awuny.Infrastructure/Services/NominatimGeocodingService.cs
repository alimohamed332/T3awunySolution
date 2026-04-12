using AutoMapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using T3awuny.Application.Contracts;
using T3awuny.Application.DTOs.Address;

namespace T3awuny.Infrastructure.Services
{
    public class NominatimGeocodingService : IGeocodingService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IConfigurationSection _nominatimSettings;
        private readonly IMapper _mapper;
        public NominatimGeocodingService(HttpClient httpClient, IConfiguration configuration, IMapper mapper)
        {
            _httpClient = httpClient;
            // Required by Nominatim terms of service
            _httpClient.DefaultRequestHeaders.UserAgent
                .ParseAdd("Taawuny-App/1.0");
            _configuration = configuration;
            _nominatimSettings = _configuration.GetSection("Nominatim");
            _mapper = mapper;         
        }

        public async Task<AddressDetailsDto> ReverseGeocodeAsync(double latitude, double longitude)
        {
            // In NominatimGeocodingService, add delay between calls if needed
            // add delay here to respect rate limiting or Nominatim TOS
            //await Task.Delay(1000); make it active if needed ,  but we will not need it 
            var url = $"{_nominatimSettings["EndpointUrl"]}" +
                      $"?lat={latitude}&lon={longitude}&format={_nominatimSettings["Format"]}&addressdetails={_nominatimSettings["AddressDetails"]}";

            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();  //return if the error occur ?

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<NominatimResponse>(json);
            // implement mapping here
            var addressDetails = _mapper.Map<AddressDetailsDto>(result);

            return addressDetails;
        }
    }
    
}
