def write_lalafo_item_url(path:str, url:str):
    with open(path, 'w', encoding='utf-8') as f:
        f.write(url)