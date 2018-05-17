import { DataStoreType } from '../enums/data-store-type';

import { ActionResponse } from '../models/action-response';
import { AzureLocation } from '../models/azure-location';

import { ViewModelBase } from '../services/view-model-base';

export class Blob extends ViewModelBase {
    azureLocations: AzureLocation[] = [];
    storageAccountSuffix: string = '.blob.core.windows.net/';
    storageAccountName: string = '';
    storageAccountPath: string = '';
    storageAccountKey: string = '';
    storageAccountContainer: string = '';
    storageAccountDirectory: string = '';

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

        response = await await this.validateBlobStorage();

        if (response && response.IsSuccess) {
            this.MS.DataStore.addToDataStore('StorageAccountName', this.storageAccountName, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('StorageAccountKey', this.storageAccountKey, DataStoreType.Private);
            this.MS.DataStore.addToDataStore('StorageAccountContainer', this.storageAccountContainer, DataStoreType.Public);
            this.MS.DataStore.addToDataStore('StorageAccountDirectory', this.storageAccountDirectory, DataStoreType.Public);

            isSuccess = true;
        } else {
            isSuccess = false;
        }

        return isSuccess;
    }

    async onValidate(): Promise<boolean> {

        this.onInvalidate();

        this.storageAccountName = this.storageAccountName.toLocaleLowerCase();

        let blobstorageResponse: ActionResponse = await this.validateBlobStorage();
        if (blobstorageResponse.IsSuccess) {
            this.setValidated();
        } else {
            this.onInvalidate();
            this.MS.ErrorService.set(blobstorageResponse.ExceptionDetail.FriendlyErrorMessage, blobstorageResponse.ExceptionDetail.AdditionalDetailsErrorMessage);
        }

        this.isValidated = this.isValidated && await super.onValidate();

        return this.isValidated;
    }

    private async validateBlobStorage(): Promise<ActionResponse> {
        let body: any = this.getBody();
        return await this.MS.HttpService.executeAsync('Microsoft-ValidateConnectionToBlobStorage', body)
    }

    private getBody(): any {
        let body: any = {};

        body.useImpersonation = this.useImpersonation;
        body['StorageAccountName'] = this.storageAccountName;
        body['StorageAccountKey'] = this.storageAccountKey

        this.spliPath();

        body['StorageAccountContainer'] = this.storageAccountContainer;
        body['StorageAccountDirectory'] = this.storageAccountDirectory;

        return body;
    }

    private spliPath(): any {

        let path = this.storageAccountPath.split('/');

        if (path.length > 1) {
            this.storageAccountContainer = path[0];
            this.storageAccountDirectory = path[1];
        } else {
            this.storageAccountContainer = path[0];
            this.storageAccountDirectory = '';
        }
    }
}