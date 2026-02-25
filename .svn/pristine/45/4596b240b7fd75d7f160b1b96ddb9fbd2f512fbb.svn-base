using Microsoft.EntityFrameworkCore.Migrations;

public class TableTrigger
{
    // concurrent write 
    public static void CreateTrigger(MigrationBuilder migrationBuilder)
    {
        // account
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetAccountTimestampOnUpdate
                    AFTER UPDATE ON Account
                    BEGIN
                        UPDATE Account
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetAccountTimestampOnInsert
                    AFTER INSERT ON Account
                    BEGIN
                        UPDATE Account
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

        //Advertising
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetAdvertisingTimestampOnUpdate
                    AFTER UPDATE ON Advertising
                    BEGIN
                        UPDATE Advertising
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetAdvertisingTimestampOnInsert
                    AFTER INSERT ON Advertising
                    BEGIN
                        UPDATE Advertising
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
                
          //AdvertisingAnalysis
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetAdvertisingAnalysisTimestampOnUpdate
                    AFTER UPDATE ON AdvertisingAnalysis
                    BEGIN
                        UPDATE AdvertisingAnalysis
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetAdvertisingAnalysisTimestampOnInsert
                    AFTER INSERT ON AdvertisingAnalysis
                    BEGIN
                        UPDATE AdvertisingAnalysis
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
                
        //Category
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetCategoryTimestampOnUpdate
                    AFTER UPDATE ON Category
                    BEGIN
                        UPDATE Category
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetCategoryTimestampOnInsert
                    AFTER INSERT ON Category
                    BEGIN
                        UPDATE Category
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

        //DynamicData
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetDynamicDataTimestampOnUpdate
                    AFTER UPDATE ON DynamicData
                    BEGIN
                        UPDATE DynamicData
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetDynamicDataTimestampOnInsert
                    AFTER INSERT ON DynamicData
                    BEGIN
                        UPDATE DynamicData
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

        //EmailConfig
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetEmailConfigTimestampOnUpdate
                    AFTER UPDATE ON EmailConfig
                    BEGIN
                        UPDATE EmailConfig
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetEmailConfigTimestampOnInsert
                    AFTER INSERT ON EmailConfig
                    BEGIN
                        UPDATE EmailConfig
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

        //FileManager
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetFileManagerTimestampOnUpdate
                    AFTER UPDATE ON FileManager
                    BEGIN
                        UPDATE FileManager
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetFileManagerTimestampOnInsert
                    AFTER INSERT ON FileManager
                    BEGIN
                        UPDATE FileManager
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

        //Html5Banner
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetHtml5BannerTimestampOnUpdate
                    AFTER UPDATE ON Html5Banner
                    BEGIN
                        UPDATE Html5Banner
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetHtml5BannerTimestampOnInsert
                    AFTER INSERT ON Html5Banner
                    BEGIN
                        UPDATE Html5Banner
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        
        
        //Log
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetLogTimestampOnUpdate
                    AFTER UPDATE ON Log
                    BEGIN
                        UPDATE Log
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetLogTimestampOnInsert
                    AFTER INSERT ON Log
                    BEGIN
                        UPDATE Log
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

        //Partner
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetPartnerTimestampOnUpdate
                    AFTER UPDATE ON Partner
                    BEGIN
                        UPDATE Partner
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetPartnerTimestampOnInsert
                    AFTER INSERT ON Partner
                    BEGIN
                        UPDATE Partner
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

        //Silde
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetSildeTimestampOnUpdate
                    AFTER UPDATE ON Silde
                    BEGIN
                        UPDATE Silde
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetSildeTimestampOnInsert
                    AFTER INSERT ON Silde
                    BEGIN
                        UPDATE Silde
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");


        //LoginHistory
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetLoginHistoryTimestampOnUpdate
                    AFTER UPDATE ON LoginHistory
                    BEGIN
                        UPDATE LoginHistory
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetLoginHistoryTimestampOnInsert
                    AFTER INSERT ON LoginHistory
                    BEGIN
                        UPDATE LoginHistory
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

        //Post
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetPostTimestampOnUpdate
                    AFTER UPDATE ON Post
                    BEGIN
                        UPDATE Post
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetPostTimestampOnInsert
                    AFTER INSERT ON Post
                    BEGIN
                        UPDATE Post
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        //LikePost
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetLikePostTimestampOnUpdate
                    AFTER UPDATE ON LikePost
                    BEGIN
                        UPDATE LikePost
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetLikePostTimestampOnInsert
                    AFTER INSERT ON LikePost
                    BEGIN
                        UPDATE LikePost
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");

        //WebAnalysis
        migrationBuilder.Sql(
                       @"
                    CREATE TRIGGER IF NOT EXISTS SetWebAnalysisTimestampOnUpdate
                    AFTER UPDATE ON WebAnalysis
                    BEGIN
                        UPDATE WebAnalysis
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
        migrationBuilder.Sql(
                @"
                    CREATE TRIGGER IF NOT EXISTS SetWebAnalysisTimestampOnInsert
                    AFTER INSERT ON WebAnalysis
                    BEGIN
                        UPDATE WebAnalysis
                        SET Timestamp = randomblob(8)
                        WHERE rowid = NEW.rowid;
                    END
                ");
    }
}