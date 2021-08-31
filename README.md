# Azure Function + Service Bus Performance Sample

This application tests azure functions with azure service bus to try and get high performance throughput. It includes terraform scripts and azure function code. 

## Structure

Client |> 
Http Function |> 
Service Bus Queue |> 
Service Bus Trigger |> 
Service Bus Queue |> 
Service Bus Trigger |> 
Cosmos DB