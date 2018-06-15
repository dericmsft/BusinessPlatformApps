import { AzureLocation } from '../models/azure-location';
import { ViewModelBase } from '../services/view-model-base';
import { DataStoreType } from '../enums/data-store-type';
import { ActionResponse } from '../models/action-response';
//import { ActionResponse } from '../models/action-response';

export class DatabricksWorkspace extends ViewModelBase {
    azureLocations: AzureLocation[] = [];

    databricksToken: string = '';
    databricksClusterName: string = '';

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

    async launchWorkspace(): Promise<void> {
        let databricksUrlResponse: ActionResponse = await this.MS.HttpService.executeAsync('Microsoft-GetDatabricksAuthUri');

        window.open(
            databricksUrlResponse.Body["value"],
            '_blank' // <- This is what makes it open in a new window.
        );
    }

    async onLoaded(): Promise<void> {
        this.onInvalidate();
    }

    async onNavigatingNext(): Promise<boolean> {
        let isSuccess: boolean = true;

        return isSuccess;
    }

    async onValidate(): Promise<boolean> {
        this.onInvalidate();

        if (this.databricksToken.length < 3) {
            this.MS.ErrorService.message = "Invalid Token";
            this.isValidated = false;
        } else {
            this.isValidated = await this.validateWorkspaceAvailability();
        }

        this.showValidation = this.isValidated;

        return this.isValidated;
    }

    private async validateWorkspaceAvailability(): Promise<boolean> {
        this.MS.DataStore.addToDataStore('AzureTokenDatabricks', this.databricksToken, DataStoreType.Private);
        return await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-CheckDatabricksWorkspaceExists', );
    }
}