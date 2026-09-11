import numpy as np

def load_raw_heightmap(path, resolution):
    data = np.fromfile(path, dtype='<u2')

    expected = resolution * resolution

    if len(data) != expected:
        raise ValueError(
            f"{path}: expected {expected} height values, "
            f"but found {len(data)}"
        )

    heightmap = data.reshape((resolution, resolution))

    heightmap = np.flipud(heightmap)

    return heightmap
def save_raw_heightmap(path, heightmap):
    raw_data = np.flipud(heightmap)

    raw_data.astype('<u2').tofile(path)

result1 = None
result2 = None

def modify_left_right(heightmap1, heightmap2):
    result1 = heightmap1.copy()
    result2 = heightmap2.copy()

    right_column = result1[:, -1].copy()

    result2[:, 0] = right_column

    return result1, result2


def modify_top_bottom(heightmap1, heightmap2):
    result1 = heightmap1.copy()
    result2 = heightmap2.copy()

    bottom_row = result1[-1, :].copy()

    result2[0, :] = bottom_row

    return result1, result2

path1 = "venusMapMod_1_8.raw"
path2 = "venusMapMod_4_8.raw"

heightmap1 = load_raw_heightmap(path1, 257)
heightmap2 = load_raw_heightmap(path2, 257)

#result1, result2 = modify_left_right(heightmap2, heightmap1) # fist is left, second is right
result1, result2 = modify_top_bottom(heightmap1, heightmap2) # first is top, second is bottom

save_raw_heightmap("results/" + path1, result1)
save_raw_heightmap("results/" + path2, result2)