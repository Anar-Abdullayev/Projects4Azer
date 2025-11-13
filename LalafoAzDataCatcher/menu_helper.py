from datetime import date, timedelta, datetime

def print_start():
    title = "LALAFO.AZ BOT"
    padding = 6
    width = len(title) + padding * 2

    print("*" * width)
    print("*" + " " * (width - 2) + "*")
    print("*" + " " * (padding - 1) + title + " " * (padding - 1) + "*")
    print("*" + " " * (width - 2) + "*")
    print("*" * width)


def get_day_choice():
    while True:
        try:
            days = int(input("Neçə gün fərq ilə axtarılsın? (0 - bugün, 1 - dünən, və s.): "))
        except ValueError:
            print("Sadəcə rəqəm daxil edə bilərsən.")
            continue

        target_date = date.today() - timedelta(days=days)
        formatted_date = target_date.strftime("%d.%m.%Y")
        target_timestamp = int(datetime.combine(target_date, datetime.min.time()).timestamp())

        confirmation = input(f"Bugündən {formatted_date} tarixədək olan dataları çəksin? (y - hə/n - yox): ").strip().lower()
        if confirmation in ("y", "yes"):
            return target_timestamp
        print("Yenidən cəhd edək...")