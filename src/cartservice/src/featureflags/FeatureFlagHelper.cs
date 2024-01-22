// Copyright The OpenTelemetry Authors
// SPDX-License-Identifier: Apache-2.0
using System;
using System.Threading.Tasks;
using Oteldemo;
using Microsoft.Extensions.Logging;


namespace cartservice.featureflags;

public class FeatureFlagHelper
{
    private static readonly Random Random = new();
    private readonly FeatureFlagService.FeatureFlagServiceClient _featureFlagServiceClient;
    private readonly ILogger<FeatureFlagHelper> _logger; // Logger instance

    public FeatureFlagHelper(ILogger<FeatureFlagHelper> logger) // Ensure the parameter is correctly defined
    {
        _logger = logger; // Correctly assign the logger

        var featureFlagServiceAddress = Environment.GetEnvironmentVariable("FEATURE_FLAG_GRPC_SERVICE_ADDR");
        _logger.LogInformation("featureFlagServiceAddress is {featureFlagServiceAddress}", featureFlagServiceAddress);
        if (string.IsNullOrEmpty(featureFlagServiceAddress))
        {
            _featureFlagServiceClient = null;
        }
        else
        {
            var featureFlagServiceUri = new Uri($"http://{featureFlagServiceAddress}");
            var channel = Grpc.Net.Client.GrpcChannel.ForAddress(featureFlagServiceUri);
            _featureFlagServiceClient = new FeatureFlagService.FeatureFlagServiceClient(channel);
        }
    }


    public async Task<bool> GenerateCartError()
    {
        if (_featureFlagServiceClient == null)
        {
            _logger.LogInformation("GenerateCartError: Returning false due to null _featureFlagServiceClient");
            return false;
        }
        var getFlagRequest = new GetFlagRequest { Name = "cartServiceFailure" };
        var getFlagResponse = await _featureFlagServiceClient.GetFlagAsync(getFlagRequest);
        _logger.LogInformation($"GenerateCartError: Returning {getFlagResponse.Flag.Enabled}");
        return getFlagResponse.Flag.Enabled;
    }
}
