
La librairie fournit plusieurs objets dédiés à la connexions et la récupération de données dans SQL Server.

##SqlFormat
Classe contenant des méthodes statiques à utiliser pour le formattage des Types de données C# vers la syntaxe de requêtes SQL.

Par exemple : DateTimeFormat ==> convertit une date c# dans les bon texte SQL

##SqlConvert
Classe contenant des méthodes statiques pour récupérer et convertir proprement des données depuis SQL Server en type c#. Les type peuvent être nullable ou pas.

Principale méthode : To<Type>(DataRow, string, Type default) ==> convertit la colonne stirng du dataraow dans le type fourni, renvoie la valeur par défaut si null.

##ConnectionParam
Classe qui fournit les méthode pour interroger ou remplit une base de données.
