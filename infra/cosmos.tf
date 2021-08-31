resource "azurerm_cosmosdb_account" "cosmos" {
  name                = "funcsbperfcosmos"
  location            = azurerm_resource_group.service_bus.location
  resource_group_name = azurerm_resource_group.service_bus.name
  offer_type          = "Standard"
  kind                = "GlobalDocumentDB"

  capabilities {
    name = "EnableServerless"
  }

  geo_location {      
      location          = azurerm_resource_group.service_bus.location
      failover_priority = 0
  }

  consistency_policy {
    consistency_level       = "Session"
    max_interval_in_seconds = 5
    max_staleness_prefix    = 100
  }

  timeouts {
    
  }

  tags = {
      "defaultExperience" = "Core (SQL)"
      "hidden-cosmos-mmspecial" = ""
  }
}

resource "azurerm_cosmosdb_sql_database" "cosmos" {
    name                = "myDatabase"
    resource_group_name = azurerm_resource_group.service_bus.name
    account_name        = azurerm_cosmosdb_account.cosmos.name
}

resource "azurerm_cosmosdb_sql_container" "cosmos" {
    name                = "myMessages"
    resource_group_name = azurerm_resource_group.service_bus.name
    account_name        = azurerm_cosmosdb_account.cosmos.name
    database_name       = azurerm_cosmosdb_sql_database.cosmos.name
    partition_key_path  = "/id"
    default_ttl         = 300
}