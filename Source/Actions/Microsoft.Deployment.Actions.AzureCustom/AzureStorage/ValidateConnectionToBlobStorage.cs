using Microsoft.Deployment.Common.ActionModel;
using Microsoft.Deployment.Common.Actions;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Deployment.Actions.AzureCustom.AzureStorage
{

    [Export(typeof(IAction))]
    class ValidateConnectionToBlobStorage : BaseAction
    {

        public override async Task<ActionResponse> ExecuteActionAsync(ActionRequest request)
        {
            var storageAccountName = request.DataStore.GetValue("StorageAccountName");
            var storageAccountKey = request.DataStore.GetValue("StorageAccountKey");
            var storageAccountDirectory = request.DataStore.GetValue("StorageAccountDirectory");
            var containerName = request.DataStore.GetValue("StorageAccountContainer");

            CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse($"DefaultEndpointsProtocol=http;AccountName={storageAccountName};AccountKey={storageAccountKey}");
            CloudBlobClient blobClient = cloudStorageAccount.CreateCloudBlobClient();

            bool blobStorageExists = false;

            if (string.IsNullOrWhiteSpace(storageAccountDirectory))
            {
                blobStorageExists = blobStorageExists = blobClient.GetContainerReference(containerName).Exists();
            }
            else
            {
                var blobDirectory = blobClient.GetContainerReference(containerName).GetDirectoryReference(storageAccountDirectory);
                blobStorageExists = blobDirectory.ListBlobs().Count() >= 0;
            }

            if (blobStorageExists)
            {
                return new ActionResponse(ActionStatus.Success);
            }
            else
            {
                return new ActionResponse(ActionStatus.Failure);
            }
        }
    }
}
