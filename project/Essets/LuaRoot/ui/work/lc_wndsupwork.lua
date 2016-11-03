--
-- @file    ui/work/lc_wndsupwork.lua
-- @authors cks
-- @date    2016-11-03 22:34:56
-- @desc    WNDSupWork
--

local ipairs, pairs
    = ipairs, pairs
local libugui = require "libugui.cs"
local libunity = require "libunity.cs"
local UIMGR = MERequire "ui/uimgr"
local Ref
--!*以下：自动生成的回调函数*--

local function on_subproject_grpproject_ent_entproject_click(btn)
	
end

local function on_subbtm_btnwork_click(btn)
	
end

local function on_subbtm_btnsch_click(btn)
	
end

local function on_subbtm_btnmsg_click(btn)
	
end

local function on_subbtm_btnuser_click(btn)
	
end

local function on_btndata_click(btn)
	
end

local function on_btntask_click(btn)
	
end

local function init_view()
	Ref.SubProject.GrpProject.Ent.EntProject.onAction = on_subproject_grpproject_ent_entproject_click
	Ref.SubBtm.btnWork.onAction = on_subbtm_btnwork_click
	Ref.SubBtm.btnSch.onAction = on_subbtm_btnsch_click
	Ref.SubBtm.btnMsg.onAction = on_subbtm_btnmsg_click
	Ref.SubBtm.btnUser.onAction = on_subbtm_btnuser_click
	Ref.btnData.onAction = on_btndata_click
	Ref.btnTask.onAction = on_btntask_click
	UIMGR.make_group(Ref.SubProject.GrpProject, function (New, Ent)
		New.entProject.onAction = Ent.entProject.onAction
	end)
	UIMGR.make_group(Ref.SubProject.GrpProject.ent, function (New, Ent)
		New.btn.onAction = Ent.btn.onAction
	end)
	--!*以上：自动注册的回调函数*--
end

local function init_logic()
	
end

local function start(self)
	if Ref == nil or Ref.root ~= self then
		Ref = libugui.GenLuaTable(self, "root")
		init_view()
	end
	init_logic()
end

local function update_view()
	
end

local function on_recycle()
	
end

local P = {
	start = start,
	update_view = update_view,
	on_recycle = on_recycle,
}
return P

