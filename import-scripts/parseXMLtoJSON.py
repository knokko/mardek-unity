from re import ASCII
import xml.etree.ElementTree as ET
import copy
import json

power = 2**16

original_file = "DefineSprite5118_Exported.xml"
root = ET.parse(original_file).getroot()
define_sprites = root.findall(""".//*[@type="DefineSpriteTag"]""")

frameTemplate = {
        "placeObjects": [],
        "removeObjects": [],
        }
all_sprites = {}

for sprite in define_sprites[:]:
    sprite_id = sprite.get("spriteId")
    items = sprite.findall("./subTags/*")
    spriteJSON = []
    frame = copy.deepcopy(frameTemplate)

    for item in items:
        itemType = item.get("type")

        if (itemType == "FrameLabelTag"):
            frame["label"] = item.get("name") # will only consider the last label of each frame, only 3 cases have 2 labels

        elif (itemType == "PlaceObject2Tag" or itemType == "PlaceObject3Tag"):
            placeObject = {
                "depth": int(item.get("depth")),
                "id": int(item.get("characterId"))
                }
            
            matrix = item.find("matrix")
            if (matrix is not None):
                scaleX = int(matrix.get("scaleX")) / power
                scaleY = int(matrix.get("scaleY")) / power
                if (scaleX != 0 or scaleY != 0):
                    placeObject["scaleX"] = scaleX
                    placeObject["scaleY"] = scaleY

                skew0 = int(matrix.get("rotateSkew0")) / power
                skew1 = int(matrix.get("rotateSkew1")) / power
                if (skew0 != 0 or skew1 != 0):
                    placeObject["rotateSkew0"] = skew0
                    placeObject["rotateSkew1"] = skew1

                translateX = int(matrix.get("translateX")) / 20
                translateY = int(matrix.get("translateY")) / 20
                if (translateX != 0 or translateY != 0):
                    placeObject["translateX"] = translateX
                    placeObject["translateY"] = translateY
            
            color = item.find("colorTransform")
            if (color is not None):
                placeObject["rgbaAdd"] = [int(color.get("redAddTerm")), 
                                          int(color.get("greenAddTerm")), 
                                          int(color.get("blueAddTerm")),
                                          int(color.get("alphaAddTerm"))]
                placeObject["rgbaMult"] = [int(color.get("redMultTerm")),
                                           int(color.get("greenMultTerm")),
                                           int(color.get("blueMultTerm")),
                                           int(color.get("alphaMultTerm"))]

            glow = item.find(""".//*[@type="GLOWFILTER"]""")
            if (glow is not None):
                placeObject["glowBlurXandY"] = float(glow.get("blurX")) # blurX and blurY are always the same
                placeObject["glowStrengh"] = float(glow.get("strength"))
                color = glow.find("glowColor")
                placeObject["glowColorRGBA"] = [int(color.get("red")),
                                            int(color.get("green")),
                                            int(color.get("blue")),
                                            int(color.get("alpha")),]
            
            if (sprite_id == "5118"):
                clipAction = item.find(""".//*[@type="CLIPACTIONRECORD"]""")
                if (clipAction is not None):
                    action = clipAction.get("actionBytes")
                    skin = "00736b696e00"
                    i = action.find(skin)
                    if (i > -1):
                        action = action[i+len(skin):]
                        if (action[:2] == "07"):
                            placeObject["skin"] = action[2:4]
                        else:
                            ascii_string = bytes.fromhex(action).decode("ASCII", errors="ignore")
                            placeObject["skin"] = ascii_string[1:-3]

            frame["placeObjects"].append(placeObject)
            
        elif (itemType == "RemoveObject2Tag"):
            removeObject = {
                "depth": int(item.get("depth")),
                }
            frame["removeObjects"].append(removeObject)

        elif (itemType == "DoActionTag"):
            pass
            """
            action = item.get("actionBytes")
            ascii_string = bytes.fromhex(skin).decode("ASCII", errors="ignore")
            """

        elif (itemType == "SoundStreamHead2Tag"):
            pass

        elif (itemType == "ShowFrameTag"):
            spriteJSON.append(frame)
            frame = copy.deepcopy(frameTemplate)

        else:
            print("Uncaught tag type found: " + itemType)
    
    all_sprites[sprite_id] = spriteJSON

for defineSprite in all_sprites:
    with open("output/" + defineSprite + ".json", "w") as file:
        file.write(json.dumps(all_sprites[defineSprite], indent=2))

#with open("output" + ".json", "w") as file:
#    file.write(json.dumps(all_sprites, indent=2))
