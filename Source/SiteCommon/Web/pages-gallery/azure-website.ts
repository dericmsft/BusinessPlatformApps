import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';
import { AzureLocation } from '../models/azure-location';

import { ViewModelBase } from '../services/view-model-base';

export class AppService extends ViewModelBase {
    azureLocations: AzureLocation[] = [];
    appServiceSuffix: string = '.azurewebsites.windows.net/';
    appServiceName: string = '';


    checkSqlVersion: boolean = false;
    credentialTarget: string = '';
    database: string = null;
    databases: string[] = [];
    hideSqlAuth: boolean = false;
    invalidUsernames: string[] = ['admin', 'administrator', 'dbmanager', 'dbo', 'guest', 'loginmanager', 'public', 'root', 'sa'];
    isAzureSql: boolean = false;
    isCreateAzureSqlSelected: boolean = false;
    isGovAzureSql: boolean = false;
    isWindowsAuth: boolean = true;
    newSqlDatabase: string = null;
    password: string = '';
    passwordConfirmation: string = '';
    showAllWriteableDatabases: boolean = true;
    showAzureSql: boolean = true;
    showCreateAzureSqlPrompt: boolean = false;
    showCredsWhenWindowsAuth: boolean = false;
    showDatabases: boolean = false;
    showGovAzure: boolean = false;
    showNewSqlOption: boolean = false;
    showSkuS1: boolean = true;
    showSqlRecoveryModeHint: boolean = false;
    sqlInstance: string = 'ExistingSql';
    sqlLocation: string = '';
    sqlServer: string = '';
    sqlSku: string = 'S1';
    subtitle: string = '';
    title: string = '';
    useImpersonation: boolean = false;
    username: string = '';
    validateWindowsCredentials: boolean = false;
    validationTextBox: string = '';

    onInvalidate(): void {
        super.onInvalidate();
    }

    async onLoaded(): Promise<void> {
        this.onInvalidate();
    }

    async onNavigatingNext(): Promise<boolean> {
        let isSuccess: boolean = true;

        let response: ActionResponse = null;

        response = await await this.validateAppService();

        if (response && response.IsSuccess) {
            this.MS.DataStore.addToDataStore('siteName', this.appServiceName, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('AppServiceSuffix', this.appServiceSuffix, DataStoreType.Private);

            isSuccess = true;
        } else {
            isSuccess = false;
        }

        return isSuccess;
    }

    async onValidate(): Promise<boolean> {

        this.onInvalidate();

        this.appServiceName = this.appServiceName.toLocaleLowerCase();

        let appServiceResponse: ActionResponse = await this.validateAppService();
        if (appServiceResponse.IsSuccess) {
            this.setValidated();
        } else {
            this.onInvalidate();
            this.MS.ErrorService.set(appServiceResponse.ExceptionDetail.FriendlyErrorMessage, appServiceResponse.ExceptionDetail.AdditionalDetailsErrorMessage);
        }

        this.isValidated = this.isValidated && await super.onValidate();

        return this.isValidated;
    }

    private async validateAppService(): Promise<ActionResponse> {
        //let body: any = this.getBody();
        let a = new ActionResponse();
        a.IsSuccess = true;
        return a;
        //return await this.MS.HttpService.executeAsync('Microsoft-ValidateAppServiceName', body)
    }

    //private getBody(): any {
    //    let body: any = {};

    //    body.useImpersonation = this.useImpersonation;
    //    body['AppServiceName'] = this.appServiceName;
    //    //body['StorageAccountKey'] = this.storageAccountKey;
    //    //body['StorageAccountContainer'] = this.storageAccountContainer;
    //    //body['StorageAccountDirectory'] = this.storageAccountDirectory;

    //    return body;
    //}
}