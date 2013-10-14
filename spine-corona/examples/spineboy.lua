
-- This example shows simple usage of displaying a skeleton with queued animations.

local spine = require "spine-corona.spine"

local json = spine.SkeletonJson.new()
json.scale = 1
local skeletonData = json:readSkeletonDataFile("examples/spineboy/spineboy.json")

local skeleton = spine.Skeleton.new(skeletonData)
function skeleton:createImage (attachment)
	-- Customize where images are loaded.
	return display.newImage("examples/spineboy/images/" .. attachment.name .. ".png")
end
skeleton.group.x = 150
skeleton.group.y = 325
skeleton.flipX = false
skeleton.flipY = false
skeleton.debug = true -- Omit or set to false to not draw debug lines on top of the images.
skeleton.debugAabb = true
skeleton:setToSetupPose()

local bounds = spine.SkeletonBounds.new()

-- AnimationStateData defines crossfade durations between animations.
local stateData = spine.AnimationStateData.new(skeletonData)
stateData:setMix("walk", "jump", 0.2)
stateData:setMix("jump", "walk", 0.4)

-- AnimationState has a queue of animations and can apply them with crossfading.
local state = spine.AnimationState.new(stateData)
state:setAnimationByName(0, "drawOrder")
state:addAnimationByName(0, "jump", false, 0)
state:addAnimationByName(0, "walk", true, 0)

state.onStart = function (trackIndex)
	print(trackIndex.." start: "..state:getCurrent(trackIndex).animation.name)
end
state.onEnd = function (trackIndex)
	print(trackIndex.." end: "..state:getCurrent(trackIndex).animation.name)
end
state.onComplete = function (trackIndex, loopCount)
	print(trackIndex.." complete: "..state:getCurrent(trackIndex).animation.name..", "..loopCount)
end
state.onEvent = function (trackIndex, event)
	print(trackIndex.." event: "..state:getCurrent(trackIndex).animation.name..", "..event.data.name..", "..event.intValue..", "..event.floatValue..", '"..(event.stringValue or "").."'")
end

local lastTime = 0
local touchX = 999999
local touchY = 999999
local headSlot = skeleton:findSlot("head")
Runtime:addEventListener("enterFrame", function (event)
	-- Compute time in seconds since last frame.
	local currentTime = event.time / 1000
	local delta = currentTime - lastTime
	lastTime = currentTime

	-- Bounding box hit detection.
	bounds:update(skeleton, true)
	if bounds:containsPoint(touchX, touchY) then
		headSlot.g = 0;
		headSlot.b = 0;
	else
		headSlot.g = 1;
		headSlot.b = 1;
	end

	-- Update the state with the delta time, apply it, and update the world transforms.
	state:update(delta)
	state:apply(skeleton)
	skeleton:updateWorldTransform()
end)

Runtime:addEventListener("touch", function (event)
	if event.phase ~= "ended" and event.phase ~= "cancelled" then
		-- Make the coordinates relative to the skeleton's group.
		touchX = event.x - skeleton.group.x
		touchY = skeleton.group.y - event.y
	else
		touchX = 999999
		touchY = 999999
	end
end)
