import { ViewModelBase } from '../services/view-model-base';
import { DataStoreType } from "../enums/data-store-type";

export class Invite extends ViewModelBase {

    emailAddress: string = '';

    async connect(): Promise<void> {
        this.MS.DataStore.addToDataStore("InviteEmailAddress", this.emailAddress, DataStoreType.Any);
        this.MS.DataStore.addToDataStore("InviteRedirect", window.location.href, DataStoreType.Any);
        let response: string = await this.MS.HttpService.getExecuteResponseAsync("Microsoft-TenantInvitation");
        window.location.href = response;
    }

    async onLoaded(): Promise<void> {
        super.onLoaded();

        let response: string = await this.MS.HttpService.getExecuteResponseAsync("Microsoft-TenantUserCheck");

        if (response === "true") {
            this.setValidated();
        }
    }
}