import os

folder = r"C:\Github\MARDEK-Engine\MARDEK Engine\Assets\Sprites\BattleModels\Imported Shapes"
files = os.listdir(folder)

for filename in files[:]:
    if (filename[-4:] != ".svg"):
        continue

    file_content = ""
    filepath = os.path.join(folder, filename)
    with open(filepath, 'r') as file:
        file_content = file.read()
        
    transform_separator = "<g transform="
    split1 = file_content.split(transform_separator)
    split2 = split1[1].split(")")
    split3 = split2[0].split(",")

    new_file_content = split1[0] + transform_separator

    for value in split3[:4]:
        new_file_content += value + ","
    new_file_content += " 0.0, 0.0"

    for value in split2[1:]:
        new_file_content += ")" + value

    with open(filepath, 'w') as file:
        file.write(new_file_content)