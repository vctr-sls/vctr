import os
import pg8000
from mysql import connector


def get_kwargs(connection_str):
    return dict(entry.split('=') for entry in connection_str.split(';'))


def main():
    CREATOR_GUID = os.getenv('CREATOR_GUID')

    msqlDb = connector.connect(
        **get_kwargs(os.getenv('CONNECTIONSTRING_MYSQL')))

    pgDb = pg8000.connect(
        **get_kwargs(os.getenv('CONNECTIONSTRING_PSQL'))

    cursor = msqlDb.cursor()
    cursor.execute(
        """
        SELECT `GUID`, `RootURL`, `ShortIdent`, `MaxUses`,
            `IsActive`, `Expires`, `IsPermanentRedirect`,
            `CreationDate`
        FROM `ShortLinks`;
        """)
    res = cursor.fetchall()

    for r in res:
        pgDb.run(
            """
            INSERT INTO public."Links"
            (
                "Guid", "Ident", "Destination",
                "CreatorGuid", "Enabled", "PermanentRedirect",
                "TotalAccessLimit",
                "Expires", "Created",
                "LastAccess", "AccessCount", "UniqueAccessCount"
            )
            VALUES
            (
                :guid, :ident, :destination,
                :creatorguid, :enabled, :permanent,
                :totalaccesslimit,
                :expires, :created,
                '0001-01-01 00:00:00', 0, 0
            )
            """,
            guid=r[0], ident=r[2], destination=r[1],
            creatorguid=CREATOR_GUID, enabled=(r[4] > 0), permanent=(r[6] > 0),
            totalaccesslimit=r[3],
            expires=r[5], created=r[7])

    pgDb.commit()


if __name__ == '__main__':
    main()
