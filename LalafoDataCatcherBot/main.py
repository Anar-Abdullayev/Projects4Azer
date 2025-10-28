import time
from database_service import DatabaseService
from Contstants import BASE_PAGE_URL
from LalafoHelper import get_cookies, fetch_api_page, fetch_details_page, print_lalafo_item, download_images
from menu_helper import print_start, get_day_choice
from time_service import is_old_date, timestamp_to_string


def main():
    print_start()
    choice = get_day_choice()
    db = DatabaseService()
    cookies = get_cookies(BASE_PAGE_URL)
    page_number = 1
    while True:
        print(f"\nFetching API page {page_number}...")
        page_items = fetch_api_page(cookies, page_number)
        page_number+=1
        for i, item in enumerate(page_items, start=1):
            item_id = item.get("id")
            created_at = item.get("created_time")
            is_vip = item.get("is_vip")
            print(f"{i}. Started {item_id}")
            is_exist = db.property_exists(str(item_id))
            if is_old_date(created_at, choice):
                print('old found')
                print(timestamp_to_string(created_at))
                if is_vip == True:
                    print('vip found')
                    continue
                else:
                    print("Property create time is older than required. End of program")
                    return
            if is_exist:
                print(f"{item_id} already exists. Skipping...")
                continue
            item_detail = fetch_details_page(cookies, item_id)
            print_lalafo_item(item_detail)
            download_images(item_detail)
            db.insert_property_id(item_id, item_detail)
            time.sleep(2)
        time.sleep(1)

if __name__ == "__main__":
    main()
