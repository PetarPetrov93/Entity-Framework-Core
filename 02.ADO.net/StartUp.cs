using _02.VillainName;
using Microsoft.Data.SqlClient;
using System.Runtime.CompilerServices;
using System.Text;

namespace _02.VillainNames
{
    public class StartUp
    {
        public static StringBuilder StringBuilder { get; private set; }

        //when using the async the return value of the method is always Task/Task<T> - all mthods below should be awaitted
        async static Task Main(string[] args)
        {
            // using "using" here states that the connection will be closed and disposed automnatically by the program when needed
            // this is how we connect to the given DB (Config.ConnectionString) is a static class created by us which holds a const property
            //"ConnectionString" which has a string value of the connection string. We can directly put the connection string in the new() as well
            await using SqlConnection connection = new(Config.ConncectionString);
            
            //This is how we actually open a connection, after executing this command we should be connected to the DB
            await connection.OpenAsync();

            //Task 2
            // This is where we will get the final result from the function and since it returns Task<string> we can pass the value to a string
            //UNCOMMENT: string resultTask2 = await GetAllVilliansWithTheirMinionsAsync(connection);
            //pringing the result
            // UNCOMMENT: Console.WriteLine(resultTask2);

            //Task 3 UNCOMMENT:
            //int villainId = int.Parse(Console.ReadLine());

            //string resultTask3 = await GetVillainWithMinionsByIdAsync(connection, villainId);
            //await Console.Out.WriteLineAsync(resultTask3);

            //Task 4
            string[] minionInfo = Console.ReadLine().Split(": ", StringSplitOptions.RemoveEmptyEntries);
            string[] villainInfo = Console.ReadLine().Split(": ", StringSplitOptions.RemoveEmptyEntries);

            string result = await AddNewMinionAsync(connection, minionInfo[1], villainInfo[1]);

            Console.WriteLine(result);
        }

        //Method for Task 2
        async static Task<string> GetAllVilliansWithTheirMinionsAsync(SqlConnection sqlConnection)
        {
            StringBuilder sb = new StringBuilder();
            // by using SqlCommand we use as arguments the actual SQL query and then the connection string
            SqlCommand query = new SqlCommand(SqlQueries.GetAllVillainsAndCountOfTheirMinions, sqlConnection);

            //since we will receive one (or many rows) with many(or one) column we should use SqlDataReader in order to be able to read each row
            //However in order for the reader to actually read the data which it stores, we need to use .Read() method and loop through the results
            SqlDataReader reader = await query.ExecuteReaderAsync();
            while(reader.Read())
            {
                string villainName = (string)reader["name"];
                int minionsCount = (int)reader["MinionsCount"];

                // since there could be many rows as results we use stringbuilder to get the values and append them each on a new row in the sb
                sb.AppendLine($"{villainName} - {minionsCount}");
            }
            
            return sb.ToString().TrimEnd();
        }

        //Method for Task 3
        async static Task<string> GetVillainWithMinionsByIdAsync(SqlConnection sqlConnection, int villainId)
        {
            StringBuilder sb = new StringBuilder();

            SqlCommand getVillainNameCmd = new SqlCommand(SqlQueries.GetVillainNameById, sqlConnection);
            getVillainNameCmd.Parameters.AddWithValue("@Id", villainId);

            //when we know that the result of the query will be a single value we use object and we use ExecuteScalar
            object? villainNameObj = await getVillainNameCmd.ExecuteScalarAsync();
            if (villainNameObj is null)
            {
                return $"No villain with ID {villainId} exists in the database.";
            }

            string villainName = (string)villainNameObj;

            SqlCommand getAllMinions = new SqlCommand(SqlQueries.GetAllMinionsByVillainId, sqlConnection);
            getAllMinions.Parameters.AddWithValue("@Id", villainId);

            //when we know that the result of the query will be more than a single value we use SqlDataReader and ExecuteReader
            SqlDataReader minionsReader = await getAllMinions.ExecuteReaderAsync();

            sb.AppendLine($"Villain: {villainName}");
                
            // .HasRows on SqlReader checks if there are any rows or it's empty
            if (!minionsReader.HasRows)
            {
                sb.AppendLine("no minions");
            }
            else
            {   
                // Read means moves the cursor to the next row and if there is no next row it returns false
                while (minionsReader.Read())
                {
                    //this is how we extract information from the row
                    long rowNum = (long)minionsReader["RowNum"];
                    string minionName = (string)minionsReader["Name"];
                    int age = (int)minionsReader["Age"];

                    sb.AppendLine($"{rowNum}. {minionName} {age}");
                }
            }
            return sb.ToString().TrimEnd();
        }

        //Method for Task 4
        async static Task<string> AddNewMinionAsync(SqlConnection sqlConnection, string minionInfo, string villainName)
        {
            StringBuilder sb = new StringBuilder();

            string[] minionArgs = minionInfo.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string minionName = minionArgs[0];
            int minionAge = int.Parse(minionArgs[1]);
            string townName = minionArgs[2];

            //Check if given Town exists and if it does not, we should add it (We are adding the town with a BeginTransaction method)
            SqlTransaction transaction = sqlConnection.BeginTransaction();
            try
            {
                // THIS IS FOR THE TOWN PART OF THE TASK
                SqlCommand getTownIdCmd = new SqlCommand(SqlQueries.GetTwonIdBtName, sqlConnection, transaction);
                getTownIdCmd.Parameters.AddWithValue("@townName", townName);

                object? townIdObj = await getTownIdCmd.ExecuteScalarAsync();
                if (townIdObj is null) // alternatively we can use .HasValue here, it is the same
                {
                    SqlCommand addNewTown = new SqlCommand(SqlQueries.AddNewTown, sqlConnection, transaction);
                    addNewTown.Parameters.AddWithValue("@townName", townName);

                    //Queries which do not return a result and just modify(change) the DB (like INSERT or DELETE) are executed as NonQuery
                    await addNewTown.ExecuteNonQueryAsync();

                    townIdObj = await getTownIdCmd.ExecuteScalarAsync();

                    sb.AppendLine($"Town {townName} was added to the database.");
                }
                int? townId = (int?)townIdObj;

                //THIS IS FOR THE VILLAIN PART OF THE TASK - I COULD HAVE EXTRACTED THE DIFFERENT PARTS INTO METHODS BUT I DECIDET TO PROCEED LIKE THAT
                SqlCommand getVillainId = new SqlCommand(SqlQueries.GetVillainIdByName, sqlConnection, transaction);
                getVillainId.Parameters.AddWithValue("@Name", villainName);

                int? villainId = (int?) await getVillainId.ExecuteScalarAsync();
                if (!villainId.HasValue)
                {
                    SqlCommand addVillainCmd = new SqlCommand(SqlQueries.AddVillainWithDefaultEvlinessFactor, sqlConnection, transaction);
                    addVillainCmd.Parameters.AddWithValue("@villainName", villainName);

                    //this is how we actually add the new villain to the database
                    await addVillainCmd.ExecuteNonQueryAsync();

                    villainId = (int?)await getVillainId.ExecuteScalarAsync();

                    sb.AppendLine($"Villain {villainName} was added to the database");
                }

                //THS IS FOR THE MINION PART OF THE TASK
                SqlCommand addMinionCmd = new SqlCommand(SqlQueries.AddNewMinion, sqlConnection, transaction);
                addMinionCmd.Parameters.AddWithValue("@name", minionName);
                addMinionCmd.Parameters.AddWithValue("@age", minionAge);
                addMinionCmd.Parameters.AddWithValue("@townId", townId);

                //we are adding the new minion
                await addMinionCmd.ExecuteNonQueryAsync();

                //now we need to get the Id of the new minion
                SqlCommand getMinionId = new SqlCommand(SqlQueries.GetMinionIdByName, sqlConnection, transaction);
                getMinionId.Parameters.AddWithValue("@Name", minionName);

                int? minionId = (int?)await getMinionId.ExecuteScalarAsync();

                //NOW WE NEED TO MAP THE NEWLY CREATED MINION TO THE GIVEN VILLAIN (TO MAP THE IDs INTO THE CONNECTING TABLE)
                SqlCommand addMinionToVillainCmd = new SqlCommand(SqlQueries.AddMinionToTheVillain, sqlConnection, transaction);
                addMinionToVillainCmd.Parameters.AddWithValue("@minionId", minionId);
                addMinionToVillainCmd.Parameters.AddWithValue("@villainId", villainId);

                await addMinionToVillainCmd.ExecuteNonQueryAsync();

                sb.AppendLine($"Successfully added {minionName} to be minion of {villainName}.");

                // if all is well in the "try" we save the changes to the DB
                await transaction.CommitAsync();

            }
            catch (Exception)
            {
                //if "try" does not succeed then we rollback the transaction and do not change the Database
                await transaction.RollbackAsync();
            }

            return sb.ToString().TrimEnd();
        }
    }   
}
