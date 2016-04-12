# TraceEvents
Aspectize Extension to trace all Application events

An event may be any action a client takes in your application; this can be subscription, page viewed, button clicked, sales, any type of event of your app.

## 1 - Configuration

You need a DataBaseService, either Azure Storage or SQL Server.

Add TraceEvents as Shared Application: in your application definition file, add TraceEvents in the Directories list :
```javascript
app.Directories = "TraceEvents";
```
Compile your project.

Create a configured service: in your service definition file, add the following lines:

```javascript
var myTraceEventsService = Aspectize.ConfigureNewService('MyTraceEventsService', aas.ConfigurableServices.TraceEventsConfigurableService);
myTraceEventsService.DataServiceName = 'MyDataService'; 
```

MyDataService is the name of your DataBaseService, in which you want to store your traces.

## 2 - Usage

Call the Events Command to trace an event:
- eventName: one or several event names, separated by |
- eventValue: optional value associated to an event
- userId: id or any unique string that identify the user
- info: any information associated to the event

### a/ On the Server side

```
ExecutingContext.ExecuteCommandAsync("MyTraceEventsService.Events", eventName, eventValue, userId, info);
```

### b/ On the client side

```javascript
var cmdTrace = Aspectize.Host.PrepareCommand();

cmdTrace.Attributes.aasAsynchronousCall = true;
cmdTrace.Call('Server/MyTraceEventsService.Events', eventName, value, userId, info);
```

## 3 - Reporting



