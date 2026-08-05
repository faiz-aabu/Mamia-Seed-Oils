from PIL import Image
from pathlib import Path

path = Path(r'c:\Users\DELL\Desktop\Mamia Seed Oils\MamiaSeedsOil.Web\wwwroot\images\logo\Logo.png')
img = Image.open(path).convert('RGBA')
data = img.getdata()
new_data = []
for pixel in data:
    r, g, b, a = pixel
    if a == 0:
        new_data.append((r, g, b, a))
    else:
        new_data.append(pixel)
img.putdata(new_data)
img.save(path)
print(f'Processed {path}')
