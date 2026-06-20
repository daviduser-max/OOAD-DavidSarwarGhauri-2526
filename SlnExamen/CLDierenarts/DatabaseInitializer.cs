using System;
using System.IO;
using System.Configuration;
using Microsoft.Data.Sqlite;

namespace CLDierenarts
{
    public static class DatabaseInitializer
    {
        private static readonly string connString = ConfigurationManager.ConnectionStrings["connStr"].ConnectionString;

        public static void InitializeDatabase()
        {
            string dbPath = "dieren.db";
            try
            {
                var builder = new SqliteConnectionStringBuilder(connString);
                if (!string.IsNullOrEmpty(builder.DataSource))
                {
                    dbPath = builder.DataSource;
                }
            }
            catch (Exception)
            {
                // Fallback to default
            }

            if (!File.Exists(dbPath))
            {
                using (SqliteConnection conn = new SqliteConnection(connString))
                {
                    conn.Open();

                    string sqlFilePath = @"C:\Users\sarwa\Downloads\DierenartsenDB.sql";
                    string sqlScript = "";

                    if (File.Exists(sqlFilePath))
                    {
                        sqlScript = File.ReadAllText(sqlFilePath);
                    }
                    else
                    {
                        sqlScript = @"
                            CREATE TABLE eigenaars (
                                id TEXT PRIMARY KEY,
                                voornaam TEXT NOT NULL,
                                achternaam TEXT NOT NULL
                            );

                            CREATE TABLE dieren (
                                id INTEGER PRIMARY KEY AUTOINCREMENT,
                                naam TEXT NOT NULL,
                                eigenaarId TEXT NOT NULL REFERENCES eigenaars(id),
                                geboortedatum TEXT NOT NULL,
                                gewicht REAL NOT NULL DEFAULT 0,
                                urgentie TEXT NOT NULL DEFAULT 'Normaal',
                                type TEXT NOT NULL,
                                ras TEXT,
                                isGevaccineerd INTEGER,
                                isOpgenomen INTEGER NOT NULL DEFAULT 0,
                                datumOpgenomen TEXT
                            );

                            INSERT INTO eigenaars (id, voornaam, achternaam) VALUES
                                ('EP001', 'Emma', 'Peeters'),
                                ('VB002', 'Lotte', 'Van den Berg'),
                                ('DS003', 'Jonas', 'De Smet'),
                                ('YB004', 'Yasmine', 'Bakir'),
                                ('TC005', 'Thomas', 'Claes');

                            INSERT INTO dieren
                                (naam, eigenaarId, geboortedatum, gewicht, urgentie, type, ras, isGevaccineerd, isOpgenomen, datumOpgenomen)
                            VALUES
                                ('Bobbie', 'EP001', '2018-03-15', 12.5, 'Normaal', 'Hond', 'Labrador', NULL, 0, NULL),
                                ('Nala', 'VB002', '2020-07-22', 4.2, 'Spoed', 'Kat', NULL, 1, 1, '2025-06-10T09:30:00'),
                                ('Max', 'DS003', '2016-11-08', 28.0, 'Spoed', 'Hond', 'Golden Retriever', NULL, 1, '2025-06-12T14:15:00'),
                                ('Luna', 'YB004', '2021-04-30', 3.8, 'Normaal', 'Kat', NULL, 0, 0, NULL),
                                ('Buddy', 'TC005', '2019-09-14', 10.3, 'Laag', 'Hond', 'Beagle', NULL, 0, NULL),
                                ('Milo', 'EP001', '2022-01-18', 5.1, 'Normaal', 'Kat', NULL, 1, 0, NULL),
                                ('Bella', 'DS003', '2017-06-25', 22.7, 'Spoed', 'Hond', 'Boxer', NULL, 1, '2025-06-14T08:00:00'),
                                ('Tiger', 'VB002', '2019-12-03', 4.9, 'Laag', 'Kat', NULL, 0, 0, NULL),
                                ('Charlie', 'YB004', '2020-08-11', 6.8, 'Normaal', 'Hond', 'Poedel', NULL, 0, NULL),
                                ('Lily', 'TC005', '2023-03-27', 3.2, 'Normaal', 'Kat', NULL, 0, 0, NULL);
                        ";
                    }

                    using (SqliteCommand comm = new SqliteCommand(sqlScript, conn))
                    {
                        comm.ExecuteNonQuery();
                    }
                }
            }
        }
    }
}
