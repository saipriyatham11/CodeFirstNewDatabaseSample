using System;
using System.Diagnostics;
using System.ServiceModel;
// ReSharper disable once RedundantUsingDirective
using System.ServiceModel.Description;

// These namespaces are found in the Microsoft.Xrm.Sdk.dll assembly
// found in the SDK\bin folder.
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using Microsoft.Xrm.Sdk.Query;
using Microsoft.Xrm.Sdk.Discovery;

// This namespace is found in Microsoft.Crm.Sdk.Proxy.dll assembly
// found in the SDK\bin folder.
using Microsoft.Crm.Sdk.Messages;

namespace Samp1
{
    public class CreateAndPublishProducts
    {
        #region Class Level Members

        /// <summary>
        /// Stores the organization service proxy.
        /// </summary>
        private OrganizationServiceProxy _serviceProxy;

        // Define the IDs needed for this sample.
        public Guid UnitGroupId;
        public Guid Product1Id;
        public Guid Product2Id;
        public Guid ProductFamilyId;
        public Guid PriceListId;
        public Guid PriceListItem1Id;
        public Guid PriceListItem2Id;
        public Guid ProductRelationId;
        public UoM Unit;

        #endregion Class Level Members

        #region How To Sample Code

        /// <summary>
        /// Create and configure the organization service proxy.
        /// Initiate creating all entity records that this sample requires.
        /// Create a product family with two child product records.
        /// Create a substitute relation between the two products.
        /// Publish the product family hierarchy, including the child records.
        /// Optionally delete any entity records that were created for this sample.
        /// </summary>
        /// <param name="serverConfig">Contains server connection information.</param>
        /// <param name="promptforDelete">When True, the user will be prompted to delete all
        /// created entities.</param>
        /// <param name="promptForDelete"></param>
        public void Run(ServerConnection.Configuration serverConfig, bool promptForDelete)
        {
            try
            {
                // Connect to the Organization service. 
                // The using statement assures that the service proxy will be properly disposed.
                using (_serviceProxy = ServerConnection.GetOrganizationProxy(serverConfig))
                {
                    // This statement is required to enable early-bound type support.
                    _serviceProxy.EnableProxyTypes();

                    // Call the method to create any data that this sample requires.
                    CreateRequiredRecords();

                    // Create a product family
                    Entity newProductFamily = new Product
                    {
                        Name = "Example Product Family",
                        ProductNumber = "PF001",
                        ProductStructure = new OptionSetValue(2)
                    };
                    ProductFamilyId = _serviceProxy.Create(newProductFamily);
                    Console.WriteLine("\nCreated {0}", newProductFamily.Name);

                    // Create couple of product records under the product family
                    Entity newProduct1 = new Product
                    {
                        Name = "Example Product 1",
                        ProductNumber = "P001",
                        ProductStructure = new OptionSetValue(1),
                        ParentProductId = new EntityReference(Product.EntityLogicalName, ProductFamilyId),
                        QuantityDecimal = 2,
                        DefaultUoMScheduleId = new EntityReference(UoMSchedule.EntityLogicalName, UnitGroupId),
                        DefaultUoMId = new EntityReference(UoM.EntityLogicalName, Unit.Id)
                    };
                    Product1Id = _serviceProxy.Create(newProduct1);

                    Console.WriteLine("Created {0} under the product family", newProduct1.Name);

                    var newProduct2 = new Product
                    {
                        Name = "Example Product 2",
                        ProductNumber = "P002",
                        ProductStructure = new OptionSetValue(1),
                        ParentProductId = new EntityReference(Product.EntityLogicalName, ProductFamilyId),
                        QuantityDecimal = 2,
                        DefaultUoMScheduleId = new EntityReference(UoMSchedule.EntityLogicalName, UnitGroupId),
                        DefaultUoMId = new EntityReference(UoM.EntityLogicalName, Unit.Id)
                    };
                    Product2Id = _serviceProxy.Create(newProduct2);

                    Console.WriteLine(@"Created {0} under the product family", newProduct2.Name);

                    // Create a price list items for the products
                    var newPriceListItem1 = new Entity
                    {
                        PriceLevelId = new EntityReference(PriceLevel.EntityLogicalName, PriceListId),
                        ProductId = new EntityReference(Product.EntityLogicalName, Product1Id),
                        UoMId = new EntityReference(UoM.EntityLogicalName, Unit.Id),
                        Amount = new Money(20)
                    };
                    PriceListItem1Id = _serviceProxy.Create(newPriceListItem1);

                    Console.WriteLine("Created price list for {0}", newProduct1.Name);

                    Entity newPriceListItem2 = new ProductPriceLevel
                    {
                        PriceLevelId = new EntityReference(PriceLevel.EntityLogicalName, PriceListId),
                        ProductId = new EntityReference(Product.EntityLogicalName, Product2Id),
                        UoMId = new EntityReference(UoM.EntityLogicalName, Unit.Id),
                        Amount = new Money(20)
                    };
                    PriceListItem2Id = _serviceProxy.Create(newPriceListItem2);

                    Console.WriteLine("Created price list for {0}", newProduct2.Name);

                    // Set the product relationship
                    // Set Example Product 1 and Example Product 2 as substitute of each other (bi-directional)
                    Entity newProductRelation = new ProductSubstitute
                    {
                        SalesRelationshipType = new OptionSetValue(3),
                        Direction = new OptionSetValue(1),
                        ProductId = new EntityReference(Product.EntityLogicalName, Product1Id),
                        SubstitutedProductId = new EntityReference(Product.EntityLogicalName, Product2Id)
                    };
                    ProductRelationId = _serviceProxy.Create(newProductRelation);

                    Console.WriteLine("Created a substitute relation between the two products.");

                    // Prompt the user whether to publish the product family and products
                    bool publishRecords = true;
                    Console.WriteLine("\nDo you want the product records published? (y/n)");
                    String answer = Console.ReadLine();
                    Debug.Assert(answer != null, "answer != null");
                    publishRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));

                    if (publishRecords)
                    {
                        PublishProductHierarchyRequest publishReq = new PublishProductHierarchyRequest
                        {
                            Target = new EntityReference(Product.EntityLogicalName, ProductFamilyId)
                        };
                        PublishProductHierarchyResponse published = (PublishProductHierarchyResponse)_serviceProxy.Execute(publishReq);
                        if (published.Results != null)
                        {
                            Console.WriteLine("Published the product records");
                        }
                    }

                    DeleteRequiredRecords(promptForDelete);
                }
            }
            catch
            {
                // You can handle an exception here or pass it back to the calling method.
                throw;
            }
        }

        /// <summary>
        /// This method creates any entity records that this sample requires.
        /// Create a unit group and price list.
        /// </summary>
        public void CreateRequiredRecords()
        {
            Console.WriteLine("Creating required records for the sample:");
            // Create a unit group.
            Entity newUnitGroup = new UoMSchedule
            {
                Name = "Example Unit Group",
                BaseUoMName = "Example Primary Unit"
            };

            UnitGroupId = _serviceProxy.Create(newUnitGroup);

            Console.WriteLine("Created {0}", newUnitGroup.Name);

            // retrieve the unit id.
            QueryExpression unitQuery = new QueryExpression
            {
                EntityName = UoM.EntityLogicalName,
                ColumnSet = new ColumnSet("uomid", "name"),
                Criteria = new FilterExpression(),
                PageInfo = new PagingInfo
                {
                    PageNumber = 1,
                    Count = 1
                }
            };
            unitQuery.Criteria.AddCondition("uomscheduleid", ConditionOperator.Equal, UnitGroupId);

            // Retrieve the unit.
            Unit = (UoM)_serviceProxy.RetrieveMultiple(unitQuery).Entities[0];

            Console.WriteLine("Ret{1}rieved {0}", Unit.Name, "ARG1");


            // Create a price list
            PriceLevel newPriceList = new PriceLevel
            {
                Name = "Example Price List"
            };
            PriceListId = _serviceProxy.Create(newPriceList);

            Console.WriteLine("Created {0}", newPriceList.Name);

            return;
        }

        /// <summary>
        /// Deletes any entity records that were created for this sample.
        /// <param name="prompt">Indicates whether to prompt the user to delete the records created in this sample.</param>
        /// </summary>
        public void DeleteRequiredRecords(bool prompt)
        {
            bool deleteRecords = true;

            if (prompt)
            {
                Console.WriteLine("\nDo you want these entity records deleted? (y/n)");
                String answer = Console.ReadLine();
                deleteRecords = (answer.StartsWith("y") || answer.StartsWith("Y"));
            }

            if (deleteRecords)
            {
                Console.WriteLine("Deleting entity records... please wait!");

                _serviceProxy.Delete(ProductSubstitute.EntityLogicalName, ProductRelationId);
                _serviceProxy.Delete(ProductPriceLevel.EntityLogicalName, PriceListItem1Id);
                _serviceProxy.Delete(ProductPriceLevel.EntityLogicalName, PriceListItem2Id);
                _serviceProxy.Delete(Product.EntityLogicalName, ProductFamilyId);
                _serviceProxy.Delete(PriceLevel.EntityLogicalName, PriceListId);
                _serviceProxy.Delete(UoMSchedule.EntityLogicalName, UnitGroupId);

                Console.WriteLine("Entity records have been deleted.");
            }
        }
        #endregion How To Sample Code

        #region Main
        /// <summary>
        /// Standard Main() method used by most SDK samples.
        /// </summary>
        /// <param name="args"></param>
        public static void Main(string[] args)
        {
            try
            {
                // Obtain the target organization's Web address and client logon 
                // credentials from the user.
                ServerConnection serverConnect = new ServerConnection();
                ServerConnection.Configuration config = serverConnect.GetServerConfiguration();

                CreateAndPublishProducts app = new CreateAndPublishProducts();
                app.Run(config, true);
            }

            catch (FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Timestamp: {0}", ex.Detail.Timestamp);
                Console.WriteLine("Code: {0}", ex.Detail.ErrorCode);
                Console.WriteLine("Message: {0}", ex.Detail.Message);
                Console.WriteLine("Plugin Trace: {0}", ex.Detail.TraceText);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
            }
            catch (System.TimeoutException ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine("Message: {0}", ex.Message);
                Console.WriteLine("Stack Trace: {0}", ex.StackTrace);
                Console.WriteLine("Inner Fault: {0}",
                    null == ex.InnerException.Message ? "No Inner Fault" : ex.InnerException.Message);
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("The application terminated with an error.");
                Console.WriteLine(ex.Message);

                // Display the details of the inner exception.
                if (ex.InnerException != null)
                {
                    Console.WriteLine(ex.InnerException.Message);

                    FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault> fe = ex.InnerException
                        as FaultException<Microsoft.Xrm.Sdk.OrganizationServiceFault>;
                    if (fe != null)
                    {
                        Console.WriteLine("Timestamp: {0}", fe.Detail.Timestamp);
                        Console.WriteLine("Code: {0}", fe.Detail.ErrorCode);
                        Console.WriteLine("Message: {0}", fe.Detail.Message);
                        Console.WriteLine("Plugin Trace: {0}", fe.Detail.TraceText);
                        Console.WriteLine("Inner Fault: {0}",
                            null == fe.Detail.InnerFault ? "No Inner Fault" : "Has Inner Fault");
                    }
                }
            }
            // Additional exceptions to catch: SecurityTokenValidationException, ExpiredSecurityTokenException,
            // SecurityAccessDeniedException, MessageSecurityException, and SecurityNegotiationException.

            finally
            {
                Console.WriteLine("Press <Enter> to exit.");
                Console.ReadLine();
            }
        }
        #endregion Main

        public Entity newProduct1 { get; set; }
    }

    public class UoM
    {
    }

    public class ServerConnection
    {
    }
}
