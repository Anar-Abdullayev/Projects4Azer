from datetime import datetime
import pyodbc
from time_service import timestamp_to_string

DRIVER_NAME = 'SQL SERVER'
DB_SERVER = r"DESKTOP-IR2K6ST\SQLEXPRESS"
DB_NAME = "LalafoDB"

CONNECTION_STRING = (
    r"DRIVER={ODBC Driver 17 for SQL Server};"
    f"SERVER={DB_SERVER};"
    f"DATABASE={DB_NAME};"
    f"Trusted_Connection=yes;"
)


class DatabaseService:
    def __init__(self):
        self._ensure_db()

    def _get_conn(self):
        return pyodbc.connect(CONNECTION_STRING)

    def _ensure_db(self):
        with self._get_conn() as conn:
            cursor = conn.cursor()
            cursor.execute("""
                IF NOT EXISTS (
                    SELECT * FROM sysobjects 
                    WHERE name='properties' AND xtype='U'
                )
                CREATE TABLE properties (
                    property_id NVARCHAR(255) PRIMARY KEY,
                    title NVARCHAR(500),
                    price NVARCHAR(255),
                    city NVARCHAR(255),
                    description NVARCHAR(MAX),
                    username NVARCHAR(255),
                    contact_number NVARCHAR(50),
                    is_vip BIT,
                    features NVARCHAR(MAX),
                    created_at DATETIME,
                    created_at_str NVARCHAR(255),
                    url NVARCHAR(MAX)
                )
            """)
            conn.commit()

    def insert_property_id(self, property_id: str, item) -> bool:
        params = item.get("params", [])
        created_at_timestamp = int(item.get("created_time", 0))

        created_at_dt = datetime.fromtimestamp(created_at_timestamp)  # real datetime
        created_at_str = timestamp_to_string(created_at_timestamp)  # dd.MM.yyyy string for NVARCHAR

        title = item.get("title", "")
        price = item.get("price", "")
        city = item.get("city", "")
        description = item.get("description", "")
        username = item.get("username", "")
        contact_number = item.get("mobile", "")
        is_vip = item.get("is_vip", False)
        url = f'https://lalafo.az{item.get("url", "")}'
        features = ', '.join(
            f'{f.get("name", "")}: {f.get("value", "")}' for f in params
        )

        try:
            with self._get_conn() as conn:
                cursor = conn.cursor()
                cursor.execute("""
                               INSERT INTO properties
                               (property_id, title, price, city, description, username, contact_number, is_vip,
                                features, created_at, created_at_str, url)
                               VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
                               """, (
                                   property_id,
                                   title,
                                   price,
                                   city,
                                   description,
                                   username,
                                   contact_number,
                                   int(is_vip),
                                   features,
                                   created_at_dt,
                                   created_at_str,
                                   url
                               ))
                conn.commit()
            return True
        except pyodbc.IntegrityError:
            return False
        except pyodbc.Error as e:
            print("DB error:", e)
            return False

    def property_exists(self, property_id: str) -> bool:
        with self._get_conn() as conn:
            cursor = conn.cursor()
            cursor.execute("SELECT 1 FROM properties WHERE property_id = ?", (property_id,))
            return cursor.fetchone() is not None

    def delete_property(self, property_id: str) -> bool:
        with self._get_conn() as conn:
            cursor = conn.cursor()
            cursor.execute("DELETE FROM properties WHERE property_id = ?", (property_id,))
            conn.commit()
            return cursor.rowcount > 0

    def list_properties(self) -> list[tuple]:
        with self._get_conn() as conn:
            cursor = conn.cursor()
            cursor.execute("SELECT property_id, title, created_at FROM properties ORDER BY created_at DESC")
            return cursor.fetchall()
