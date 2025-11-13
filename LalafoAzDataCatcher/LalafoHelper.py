import os
import re
import time
import requests
import uuid
from datetime import datetime
from playwright.sync_api import sync_playwright
from Contstants import API_URL, ITEMS_PER_PAGE, CATEGORY_ID, SORT_BY, DETAIL_API_URL
from time_service import timestamp_to_string
from file_service import write_lalafo_item_url


def get_cookies(url: str):
    """Use Playwright to get cookies including cf_clearance."""
    with sync_playwright() as p:
        browser = p.chromium.launch(headless=True)
        context = browser.new_context()
        page = context.new_page()

        print("Opening page to get cookies...")
        page.goto(url, timeout=60000)
        page.wait_for_selector("body")  # Wait until page fully loads

        cookies = context.cookies()
        cookie_dict = {c['name']: c['value'] for c in cookies}
        print("Cookies obtained:", cookie_dict)

        browser.close()
        return cookie_dict


def fetch_api_page(cookies, page_number=1):
    """Fetch a single page from the search API."""
    headers = {
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36",
        "Accept": "application/json, text/plain, */*",
        "x-request-id": f"react-client_{uuid.uuid4()}",
        "device": "pc",
        "experiment": "novalue",
        "user-hash": cookies.get("event_user_hash", ""),
        "country-id": "13",
        "language": "az_AZ",
        "Referer": API_URL
    }

    params = {
        "expand": "url",
        "per-page": ITEMS_PER_PAGE,
        "category_id": CATEGORY_ID,
        "sort_by": SORT_BY,
        "with_feed_banner": "true",
        "page": page_number
    }

    response = requests.get(API_URL, params=params, cookies=cookies, headers=headers)
    if response.status_code == 200:
        data = response.json()
        items = data.get("items", [])
        return items
    return []


def fetch_details_page(cookies, property_id: int):
    headers = {
        "User-Agent": "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/139.0.0.0 Safari/537.36",
        "Accept": "application/json, text/plain, */*",
        "x-request-id": f"react-client_{uuid.uuid4()}",
        "device": "pc",
        "experiment": "novalue",
        "user-hash": cookies.get("event_user_hash", ""),
        "country-id": "13",
        "language": "az_AZ",
        "Referer": DETAIL_API_URL
    }
    params = {
        "expand": "url",
    }
    final_url = DETAIL_API_URL + str(property_id)

    response = requests.get(final_url, params=params, cookies=cookies, headers=headers)
    if response.status_code == 200:
        data = response.json()
        return data
    else:
        return []


def print_lalafo_item(item):
    params = item.get("params", [])
    created_at = timestamp_to_string(int(item["created_time"]))
    print(f'Title: {item.get("title", "")}')
    print(f'Price: {item.get("price", "")}')
    print(f'City: {item.get("city", "")}')
    print('Features:')

    for feature in params:
        print(f'\t{feature.get("name", "")}: {feature.get("value", "")}')
    print(f'Description: {item.get("description", "")}')
    print(f'Shared by: {item.get("username", "")}')
    print(f'Contact number: {item.get("mobile", "")}')
    print(f'Created at: {created_at}')
    print(f'VIP {item.get("is_vip")}')
    print(f'VIP Elan: {"VIP" if item.get("is_vip") == True else "Not VIP"}')


def download_images(item_detail,base_dir:str = './images'):
    timestamp = int(time.time())
    urls = [item.get("original_url") for item in item_detail.get('images')]
    safe_title = sanitize_title(item_detail.get('title'))
    folder_name = f"{timestamp}-{safe_title}-{item_detail.get('id')}"
    folder_path = os.path.join(base_dir, folder_name)
    os.makedirs(folder_path, exist_ok=True)
    write_lalafo_item_url(f'{folder_path}/url.txt', f'https://lalafo.az/{item_detail.get('url')}')
    for i, url in enumerate(urls, start=1):
        try:
            response = requests.get(url, stream=True, timeout=10)
            response.raise_for_status()

            extention = os.path.splitext(url)[1]
            filename = f"image_{i}{extention}"
            filepath = os.path.join(folder_path, filename)
            with open(filepath, "wb") as f:
                for chunk in response.iter_content(chunk_size=1024):
                    f.write(chunk)

            print(f"Downloaded completed for: {filepath}")
        except Exception as e:
            print(f"Download failed for {url}: {e}")
    print(f"Download completed for: {urls}")


def sanitize_title(title: str, max_length: int = 100) -> str:
    cleaned = re.sub(r'[<>:"/\\|?*]', "", title)
    cleaned = re.sub(r"\s+", " ", cleaned)
    cleaned = cleaned.strip(" .")
    if len(cleaned) > max_length:
        cleaned = cleaned[:max_length].rstrip()
    return cleaned
