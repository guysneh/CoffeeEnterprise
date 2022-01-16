# CoffeeEnterprise (Written by Guy Sne (2022))
Coffee Factory microservice and Coffee Store API


Instruction to get the Coffee Factory and the Coffee Store run on your local machine:

1. Download and install RabbitMQ version 3.9.x  (make sure you run the installation with the admin account)
   you can use this link: https://rabbitmq.com/download.html

2. Download and install Microsoft SQL Server 2019 
   you can use this link: https://www.microsoft.com/en-us/sql-server/sql-server-downloads

3. Download and install .NET 6.0.x SDK
   you can use this link: https://dotnet.microsoft.com/en-us/download/dotnet/6.0

4. If you need to restart you computer after one or more of the above installations please do it now and then continue to the next step(5).

5. Publish the Coffee Factory/Store C# projects(do the same for both projects):
   - open the command line terminal
   - make sure you are in the same directory where the <project-name>.cs is found.
   - Type the following command:
   'dotnet publish <project-name>.csproj --self-contained false --configuration Release --output <the path in which you want the project to be published>'
   
   example:
   'dotnet publish CoffeeStore.csproj --self-contained false --configuration Release --output .\Publish'

5. The CoffeeStore project contains a setting file in its root directory named: appsettings.json
   This file conatins default settings for the message queue(RabbitMQ) and for the connection string for the database connection.
   Change it just if you need to do it due to security issues (for example using username and password or a different hostname etc.)
   If you do so please make sure that the settings for the message queue are IDENTICAL for both the CoffeeFactory(will be explained how to do so) and for the CoffeeStore.
   (otherwise they might not use the same queue for the coffees and the coffee store will not get any new coffees from the factory).
   The same goes of course for the DB connection string, make sure they are correct.
   
   - The key name for the queue name is: QueueName (in the root object).
   - The key name for the queue host name is: QueueHostName (in the root object).
   - The key name for the DB Connection string is: CoffeeStoreDB (in the ConnectionStrings node).

6. Make sure the RabbitMQ service is running on your machine.

7. Make sure the SQL Server is running on your machine.

8. Run the Coffee Factory:
   - Go the the path to which you published the coffee factory
   - If you use Windows run this command: 'CoffeeFactory.exe' 
   - Alternatively you can use this command for Linux/Mac (and for windows as well): dotnet CoffeeFactory.dll
   - If you want to change the default settings of the factory add those arguments to the run command:
   - 'CoffeeFactory.exe / dotnet CoffeeFactory.dll --every [integer] --queueName [string] --hostname [string]'
   
   For example if you decided to change the queue name to : 'coffee2000' , your queue host name to : 'https:\\some.queue.broker' 
   and you want the coffees to be produced every 2 seconds instead of the default 5 seconds, run the factory in the follwing way:
   'CoffeeFactory.exe / dotnet CoffeeFactory.dll --every 2000 --queueName coffee2000 --hostname https:\\some.queue.broker'

9. Run the Coffee Store:
   - Go the the path to which you published the coffee store
   - If you use Windows run this command: 'CoffeeStore.exe' 
   - Alternatively you can use this command for Linux/Mac (and for windows as well): dotnet CoffeeStore.dll

(you can run both projects independenly and in the order that you like)


Test the projects:

1) For the coffee factory: you should see the logs for the produced coffees on the terminal.
   Every log contain the id of the coffee and the date and time it was produced.

2) For the coffee store:
   Before you start it, you can let the coffee factory produce some coffees and stop it.
   When you run the coffee store you should be able to see those coffee in the store (even though the coffee store was not available at the producing time)
   The Coffee Store exposes a REST API with which you can test it (or even really use it! ;)

   The list of the API Endpoints:
    (you can test it with a software like Postman for example)
   
     * the default for the path base is : 'https://localhost:5001' - you should see this specified on the terminal after you run the coffee store.
     I used this path base on the API Documentation:

     1) GET  https://localhost:5001/Store/Coffees/Count -> returns a number representing the amount of the available coffees in the store.
     
     2) GET  https://localhost:5001/Store/Orders -> returns the orders that have been made in the store (default is page 0, size 20)
        Optional query parameters: 
        - 'customerName'(string) - filters the orders that belong to the given customer name
        - 'page'(integer)  - specifies the page number
        - 'size' (integer) - the specifies the page size (how much orders in a page)
        
        In every order object you should see the: Order-Id, the amount of coffees for this order(the coffees ids are not included!) and the customer name.  
     
     3) GET  https://localhost:5001/Store/Order/{order-id} -> returns a single order by id (has to be a parsable Guid)
        The information for the returned order object is the same as in the last restpoint but it specifies in addition all the coffee ids for this order.

        if the order id was not found - returns a 404(not found) response.
    
     4) GET  https://localhost:5001/Store/Coffees -> returns all the available coffees in the store (default is page 0, size 20)
        Optional query parameters: 
        - 'page'(integer)  - specifies the page number
        - 'size' (integer) - the specifies the page size (how much coffees in a page)

     5) GET  https://localhost:5001/Store/Coffee/{coffee-id} -> returns a single coffee by  id (has to be a parsable Guid)
        The differnce here is that one can also get coffees that are already been sold and therefore are not available in the store.
        (Sold coffees are not being deleted from the DB)
        
 	if the coffee id was not found - returns a 404(not found) response.

     6) POST https://localhost:5001/Store/Buy
        
	- The body that should be added to the request should look like this:

	{
		"amount": <amount of coffees to buy>,
		"customerName": "<customerName>"
	}

	- Content-Type header should be: application/json

        - if there is not enough coffees in the store the reponse will have the status code 409(conflict) and include the amount of available coffees in the store.
        - if there is enough coffees in the store the response will have the statuc code 201(created) and include all coffee ids that being sold to the customer
        - the code uses transcations in the database so in case of two customers sending simulatensouly two requests the request that being proccessed first will 'win'
          and the other customer will get a conflict(209) message (and no coffee from his/her request will be flagged as 'deleted' in the database).


Deployment:
	
	I would use some cloud services for deploying the coffee 'enterprise'.
        For example: AWS (Amazon Web Services) - the coffee store could be deployed as a Web Api or in lambda functions.
                                               - the RabbitMQ could be deployed as well(AWS work with them too ;) and we change the settings to be adapted to the new location
						 (here is the place to say that in a real world scenario the implemation of the queue would be obviously more complex,
						  therefore we would have to modify the source code as well)
					       - We could use also use the DB services of AWS to deploy the Database.

	If the enterprise is used in an internal network we can also use some web server (for example IIS 10 in windows).