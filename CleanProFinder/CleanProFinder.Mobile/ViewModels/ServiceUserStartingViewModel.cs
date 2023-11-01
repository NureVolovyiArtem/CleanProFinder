﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using CleanProFinder.Mobile.Services;
using CleanProFinder.Shared.Dto.Profile;

namespace CleanProFinder.Mobile.ViewModels;

public partial class ServiceUserStartingViewModel : ObservableObject
{
    private readonly IProviderService _providerService;

    public ServiceUserStartingViewModel(IProviderService providerService) 
    {
        _providerService = providerService;
        _serviceProviders = new ObservableCollection<ProviderPreviewDto>();
        IsRefreshing = true;
    }

    [ObservableProperty]
    ObservableCollection<ProviderPreviewDto> _serviceProviders;

    [ObservableProperty]
    private bool _isRefreshing;

    [ObservableProperty]
    private string _searchQuery;

    [RelayCommand]
    private async void LoadServiceProviders()
    {
        IsRefreshing = true;

        var response = await _providerService.GetServiceProvidersAsync();
        
        if (response.IsSuccess)
        {
            foreach (var serviceProvider in response.Result)
            {
                ServiceProviders.Add(serviceProvider);
            }
        }
        
        IsRefreshing = false;
    }

    [RelayCommand]
    private void Search()
    {

    }
}
