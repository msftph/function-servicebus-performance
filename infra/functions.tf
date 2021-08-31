resource "azurerm_storage_account" "func" {
  name                     = "storageaccountservi9818"
  resource_group_name      = azurerm_resource_group.service_bus.name
  location                 = azurerm_resource_group.service_bus.location
  account_tier             = "Standard"
  account_replication_type = "LRS"
  min_tls_version          = "TLS1_2"
  account_kind             = "StorageV2"
}

resource "azurerm_app_service_plan" "func" {
  name                = "ASP-servicebus-be92"
  location            = azurerm_resource_group.service_bus.location
  resource_group_name = azurerm_resource_group.service_bus.name
  kind                = "FunctionApp"

  sku {
    tier = "Dynamic"
    size = "Y1"
  }
}

resource "azurerm_function_app" "func" {
  name                       = "service-bus-functions"
  location                   = azurerm_resource_group.service_bus.location
  resource_group_name        = azurerm_resource_group.service_bus.name
  app_service_plan_id        = azurerm_app_service_plan.func.id
  storage_account_name       = azurerm_storage_account.func.name
  storage_account_access_key = azurerm_storage_account.func.primary_access_key

  version                    = "~3"
  enable_builtin_logging     = false
}