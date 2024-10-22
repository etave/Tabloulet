class_name InputEventHelper
extends Node

func _get_event_type(event: InputEvent) -> int:
	if (event is InputEventSingleScreenTouch):
		return 0
	if (event is InputEventSingleScreenTap):
		return 1
	if (event is InputEventSingleScreenSwipe):
		return 2
	if (event is InputEventSingleScreenLongPress):
		return 3
	if (event is InputEventSingleScreenDrag):
		return 4
	if (event is InputEventScreenTwist):
		return 5
	if (event is InputEventScreenPinch):
		return 6
	if (event is InputEventScreenCancel):
		return 7
	if (event is InputEventMultiScreenTap):
		return 8
	if (event is InputEventMultiScreenSwipe):
		return 9
	if (event is InputEventMultiScreenLongPress):
		return 10
	if (event is InputEventMultiScreenDrag):
		return 11
	return -1
