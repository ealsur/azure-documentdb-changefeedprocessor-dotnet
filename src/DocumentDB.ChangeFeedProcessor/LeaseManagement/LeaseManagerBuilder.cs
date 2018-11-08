﻿//----------------------------------------------------------------
// Copyright (c) Microsoft Corporation.  Licensed under the MIT license.
//----------------------------------------------------------------

namespace Microsoft.Azure.Documents.ChangeFeedProcessor.LeaseManagement
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.Documents.ChangeFeedProcessor.DataAccess;
    using Microsoft.Azure.Documents.ChangeFeedProcessor.Utils;

    /// <summary>
    /// Provides flexible way to build lease manager constructor parameters.
    /// For the actual creation of lease manager instance, delegates to lease manager factory.
    /// </summary>
    internal class LeaseManagerBuilder
    {
        private LeaseManagerSettings settings = new LeaseManagerSettings();
        private IChangeFeedDocumentClient client;
        private IRequestOptionsFactory requestOptionsFactory;

        public LeaseManagerBuilder WithLeaseCollection(DocumentCollectionInfo leaseCollectionLocation)
        {
            if (leaseCollectionLocation == null) throw new ArgumentNullException(nameof(leaseCollectionLocation));

            this.settings.LeaseCollectionInfo = leaseCollectionLocation.Canonicalize();
            return this;
        }

        public LeaseManagerBuilder WithLeaseDocumentClient(IChangeFeedDocumentClient leaseDocumentClient)
        {
            if (leaseDocumentClient == null) throw new ArgumentNullException(nameof(leaseDocumentClient));

            this.client = leaseDocumentClient;
            return this;
        }

        public LeaseManagerBuilder WithLeasePrefix(string leasePrefix)
        {
            if (leasePrefix == null) throw new ArgumentNullException(nameof(leasePrefix));

            this.settings.ContainerNamePrefix = leasePrefix;
            return this;
        }

        public LeaseManagerBuilder WithLeaseCollectionLink(string leaseCollectionLink)
        {
            if (leaseCollectionLink == null) throw new ArgumentNullException(nameof(leaseCollectionLink));

            this.settings.LeaseCollectionLink = leaseCollectionLink;
            return this;
        }

        public LeaseManagerBuilder WithRequestOptionsFactory(IRequestOptionsFactory requestOptionsFactory)
        {
            if (requestOptionsFactory == null) throw new ArgumentNullException(nameof(requestOptionsFactory));

            this.requestOptionsFactory = requestOptionsFactory;
            return this;
        }

        public LeaseManagerBuilder WithHostName(string hostName)
        {
            if (hostName == null) throw new ArgumentNullException(nameof(hostName));

            this.settings.HostName = hostName;
            return this;
        }

        public Task<ILeaseManager> BuildAsync()
        {
            if (this.settings.LeaseCollectionInfo == null)
                throw new InvalidOperationException(nameof(this.settings.LeaseCollectionInfo) + " was not specified");
            if (this.settings.LeaseCollectionLink == null)
                throw new InvalidOperationException(nameof(this.settings.LeaseCollectionLink) + " was not specified");
            if (this.requestOptionsFactory == null)
                throw new InvalidOperationException(nameof(this.requestOptionsFactory) + " was not specified");

            this.client = this.client ?? this.settings.LeaseCollectionInfo.CreateDocumentClient();

            var leaseManager = new DocumentServiceLeaseManager(this.settings, this.client, this.requestOptionsFactory);
            return Task.FromResult<ILeaseManager>(leaseManager);
        }
    }
}
