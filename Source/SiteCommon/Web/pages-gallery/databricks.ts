import { AzureLocation } from '../models/azure-location';
import { ViewModelBase } from '../services/view-model-base';
import { DataStoreType } from '../enums/data-store-type';

export class Databricks extends ViewModelBase {
    azureLocations: AzureLocation[] = [];

    databricksWorkspaceName: string = '';
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

    async onLoaded(): Promise<void> {
        this.onInvalidate();
    }

    async onNavigatingNext(): Promise<boolean> {
        let isSuccess: boolean = true;

        this.MS.DataStore.addToDataStore('DatabricksWorkspaceName', this.databricksWorkspaceName, DataStoreType.Public);
        this.MS.DataStore.addToDataStore('DatabricksClusterName', this.databricksClusterName, DataStoreType.Public);
        isSuccess = await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-DeplyAzureDatabricksWorkspace');

        return isSuccess;
    }

    async onValidate(): Promise<boolean> {
        this.onInvalidate();

        if (this.databricksWorkspaceName.length < 3 || this.databricksWorkspaceName.length > 63 || !/[a-z]/.test(this.databricksWorkspaceName[0]) || !/^[a-z0-9]+$/.test(this.databricksWorkspaceName)) {
            this.MS.ErrorService.message = this.MS.Translate.SSAS_INVALID_SERVER_NAME;
            this.isValidated = false;
        } else {
            this.isValidated = await this.validateWorkspaceAvailability();
        }

        this.showValidation = this.isValidated;

        return this.isValidated;
    }

    private async validateWorkspaceAvailability(): Promise<boolean> {
        let body: any = this.getBody();
        return await this.MS.HttpService.isExecuteSuccessAsync('Microsoft-CheckDatabricksWorkspaceAvailability', body);
    }

    private getBody(): any {
        let body: any = {};

        body.useImpersonation = this.useImpersonation;
        body['DatabricksWorkspaceName'] = this.databricksWorkspaceName;
        body['DatabricksClusterName'] = this.databricksClusterName;

        return body;
    }
}