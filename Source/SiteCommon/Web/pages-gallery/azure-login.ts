import { AzureConnection } from '../enums/azure-connection';
import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';

import { ViewModelBase } from '../services/view-model-base';

export class AzureLogin extends ViewModelBase {
    authToken: any = {};
    azureConnection = AzureConnection;
    azureDirectory: string = '';
    azureProviders: string[] = [];
    bapiServices: string[] = [];
    bingUrl: string = '';
    bingtermsofuse: string = '';
    connectionType: AzureConnection = AzureConnection.Organizational;
    defaultLocation: number = 23;
    oauthType: string = '';
    pricingCalculator: string = '';
    pricingCalculatorUrl: string = '';
    pricingUrl: string = '';
    pricingCost: string = '';
    selectedResourceGroup: string = `SolutionTemplate-${this.MS.UtilityService.getUniqueId(5)}`;
    selectedSubscriptionId: string = '';
    showAdvanced: boolean = false;
    showPricingConfirmation: boolean = false;
    subscriptionsList: any[] = [];

    async connect(): Promise<void> {
        this.MS.UtilityService.connectToAzure(this.oauthType, this.isConnectionMicrosoft() ? this.azureDirectory : this.MS.Translate.DEFAULT_TENANT);
    }

    async getSubscriptions(): Promise<void> {
        let subscriptions: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureSubscriptions');
        if (subscriptions.IsSuccess) {
            this.subscriptionsList = subscriptions.Body.value;
            if (!this.subscriptionsList || (this.subscriptionsList && this.subscriptionsList.length === 0)) {
                this.MS.ErrorService.message = this.MS.Translate.AZURE_LOGIN_SUBSCRIPTION_ERROR;
            } else {
                this.selectedSubscriptionId = this.subscriptionsList[0].SubscriptionId;
                this.showPricingConfirmation = this.setValidated();
                await this.MS.HttpService.executeAsync('Microsoft-PowerBiLogin');
            }
        }
    }

    isConnectionMicrosoft(): boolean {
        return this.connectionType.toString() === AzureConnection.Microsoft.toString();
    }

    async onLoaded(): Promise<void> {
        super.onLoaded();

        if (this.subscriptionsList.length > 0) {
            this.setValidated();
        } else {
            await this.MS.UtilityService.getToken(this.oauthType, async () => {
                await this.getSubscriptions();
            });
        }
    }

    async onNavigatingNext(): Promise<boolean> {
        let isSuccess: boolean = true;

        this.MS.DataStore.addToDataStore('SelectedSubscription', this.subscriptionsList.find(x => x.SubscriptionId === this.selectedSubscriptionId), DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SelectedResourceGroup', this.selectedResourceGroup, DataStoreType.Public);

        let locationsResponse: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetLocations');
        if (locationsResponse.IsSuccess) {
            this.MS.DataStore.addToDataStore('SelectedLocation', locationsResponse.Body.value[this.defaultLocation], DataStoreType.Public);
        }

        isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-CreateResourceGroup');

        for (let i = 0; i < this.azureProviders.length && isSuccess; i++) {
            isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-RegisterProvider', { AzureProvider: this.azureProviders[i] });
        }

        for (let i = 0; i < this.bapiServices.length && isSuccess; i++) {
            isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-RegisterBapiService', { BapiService: this.bapiServices[i] });
        }

        return isSuccess;
    }

    async validateResourceGroup(): Promise<boolean> {
        this.onInvalidate();

        let subscriptionObject = this.subscriptionsList.find(x => x.SubscriptionId === this.selectedSubscriptionId);
        this.MS.DataStore.addToDataStore('SelectedSubscription', subscriptionObject, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('SelectedResourceGroup', this.selectedResourceGroup, DataStoreType.Public);

        if (await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-ExistsResourceGroup')) {
            this.setValidated();
        }

        return this.isValidated;
    }
}