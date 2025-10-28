from datetime import datetime, date

def is_old_date(timestamp: int, choice_timestamp: int) -> bool:
    created_date = datetime.fromtimestamp(timestamp).date()
    choice_date = datetime.fromtimestamp(choice_timestamp).date()
    return created_date < choice_date

def timestamp_to_string(timestamp: int) -> str:
    return datetime.fromtimestamp(timestamp).strftime("%d.%m.%Y")