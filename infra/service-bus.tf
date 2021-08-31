resource "azurerm_servicebus_namespace" "service_bus" {
  name                = "funcsbperfns"
  location            = azurerm_resource_group.service_bus.location
  resource_group_name = azurerm_resource_group.service_bus.name
  sku                 = "Standard"

  tags = {
    source = "terraform"
  }
}

resource "azurerm_servicebus_queue" "service_bus_received" {
  name                = "MyReceivedMessages"
  resource_group_name = azurerm_resource_group.service_bus.name
  namespace_name      = azurerm_servicebus_namespace.service_bus.name

  enable_partitioning = true
}

resource "azurerm_servicebus_queue" "service_bus_processed" {
  name                = "MyProcessedMessages"
  resource_group_name = azurerm_resource_group.service_bus.name
  namespace_name      = azurerm_servicebus_namespace.service_bus.name

  enable_partitioning = true
}