import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';
import { D365Organization } from '../models/d365-organization';
import { MsCrmOrganization } from '../models/ms-crm-organization';

import { AzureLogin } from './azure-login';

export class MsCrmLogin extends AzureLogin {
    d365OnPremiseOrganizationName: string = '';
    d365OnPremiseUrl: string = '';
    d365OrganizationId: string = '';
    d365Organizations: D365Organization[] = [];
    d365Password: string = '';
    d365Username: string = '';
    entities: string = '';
    isScribe: boolean = false;
    msCrmOrganizationId: string = '';
    msCrmOrganizations: MsCrmOrganization[] = [];
    showAzureTrial: boolean = false;

    async connect(): Promise<void> {
        this.MS.UtilityService.connectToAzure(this.oauthType);
    }

    async d365Login(): Promise<void> {
        this.msCrmOrganizations = await this.MS.HttpService.getResponseAsync('Microsoft-CrmGetOrgs');

        if (this.msCrmOrganizations && this.msCrmOrganizations.length > 0) {
            this.msCrmOrganizationId = this.msCrmOrganizations[0].organizationId;

            let subscriptions: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetAzureSubscriptions');
            if (subscriptions.IsSuccess) {
                this.subscriptionsList = subscriptions.Body.value;
                if (!this.subscriptionsList || (this.subscriptionsList && this.subscriptionsList.length === 0)) {
                    this.MS.ErrorService.message = this.MS.Translate.AZURE_LOGIN_SUBSCRIPTION_ERROR_CRM;
                    this.showAzureTrial = true;
                } else {
                    this.selectedSubscriptionId = this.subscriptionsList[0].SubscriptionId;
                    this.showPricingConfirmation = this.setValidated();
                }
            } else {
                this.showAzureTrial = true;
            }
        } else {
            this.MS.ErrorService.message = this.MS.Translate.MSCRM_LOGIN_NO_AUTHORIZATION;
        }
    }

    async onLoaded(): Promise<void> {
        this.onInvalidate();

        this.showAzureTrial = false;

        if (!this.isScribe) {
            if (this.subscriptionsList.length > 0 && this.msCrmOrganizations.length > 0) {
                this.setValidated();
            } else {
                this.MS.UtilityService.getToken(this.oauthType, async () => {
                    await this.d365Login();
                });
            }
        }
    }

    async onNavigatingNext(): Promise<boolean> {
        let isSuccess: boolean = true;

        this.MS.DataStore.addToDataStore('Entities', this.entities, DataStoreType.Public);

        if (this.isScribe) {
            if (!this.d365OnPremiseOrganizationName && !this.d365OnPremiseUrl) {
                let d365Organization: D365Organization = this.d365Organizations.find(x => x.id === this.d365OrganizationId);
                this.MS.DataStore.addToDataStore('ConnectorUrl', d365Organization.connectorUrl, DataStoreType.Private);
                this.MS.DataStore.addToDataStore('OrganizationName', d365Organization.name, DataStoreType.Private);
                this.MS.DataStore.addToDataStore('ScribeDeploymentType', 'Online', DataStoreType.Private);
            } else {
                this.MS.DataStore.addToDataStore('ConnectorUrl', this.d365OnPremiseUrl, DataStoreType.Private);
                this.MS.DataStore.addToDataStore('OrganizationName', this.d365OnPremiseOrganizationName, DataStoreType.Private);
                this.MS.DataStore.addToDataStore('ScribeDeploymentType', 'OnPremise', DataStoreType.Private);
            }
        } else {
            let msCrmOrganization: MsCrmOrganization = this.msCrmOrganizations.find(o => o.organizationId === this.msCrmOrganizationId);

            if (msCrmOrganization) {
                this.MS.DataStore.addToDataStore('OrganizationId', msCrmOrganization.organizationId, DataStoreType.Public);
                this.MS.DataStore.addToDataStore('OrganizationName', msCrmOrganization.organizationName, DataStoreType.Public);
                this.MS.DataStore.addToDataStore('OrganizationUrl', msCrmOrganization.organizationUrl, DataStoreType.Public);

                isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-CrmGetOrganization');

                if (isSuccess) {
                    let subscriptionObject = this.subscriptionsList.find(x => x.SubscriptionId === this.selectedSubscriptionId);
                    this.MS.DataStore.addToDataStore('SelectedSubscription', subscriptionObject, DataStoreType.Public);
                    this.MS.DataStore.addToDataStore('SelectedResourceGroup', this.selectedResourceGroup, DataStoreType.Public);

                    let locationsResponse: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetLocations');
                    if (locationsResponse.IsSuccess) {
                        this.MS.DataStore.addToDataStore('SelectedLocation', locationsResponse.Body.value[23], DataStoreType.Public);
                    }

                    isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-CreateResourceGroup');

                    for (let i = 0; i < this.azureProviders.length && isSuccess; i++) {
                        isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-RegisterProvider', { AzureProvider: this.azureProviders[i] });
                    }
                }
            }
        }

        return isSuccess;
    }

    async onValidate(): Promise<boolean> {
        this.onInvalidate();

        this.MS.DataStore.addToDataStore('D365Username', this.d365Username, DataStoreType.Private);
        this.MS.DataStore.addToDataStore('D365Password', this.d365Password, DataStoreType.Private);

        if (!this.d365OnPremiseOrganizationName && !this.d365OnPremiseUrl) {
            this.d365Organizations = await this.MS.HttpService.getResponseAsync('Microsoft-GetD365Organizations');

            if (this.d365Organizations && this.d365Organizations.length > 0) {
                this.d365OrganizationId = this.d365Organizations[0].id;
                this.setValidated();
            }
        } else {
            this.setValidated();
        }

        return this.isValidated;
    }
}