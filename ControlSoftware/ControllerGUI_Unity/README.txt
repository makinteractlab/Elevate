1. How to Use

1) Saves Json File as Assets/Resources/MatrixData/CurrentMatrix.Json
2) Impleaments the function play() in Assets/Scripts/Play


2. Json File Format

{
	"board_width": 20,
	"board_height": 60,
	"board_data_list": [
	{
	"col": _,  // int value(0~19)
	"row": _, // int value(0~59)
	"step_val": _ // int value(1~11)
	},
	...
	]
} 