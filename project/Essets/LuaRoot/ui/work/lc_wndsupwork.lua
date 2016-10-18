--
-- @file    ui/work/lc_wndsupwork.lua
-- @authors zl
-- @date    2016-08-14 18:48:47
-- @desc    WNDSupWork
--

local ipairs, pairs
    = ipairs, pairs
local libugui = require "libugui.cs"
local libunity = require "libunity.cs"
local UIMGR = MERequire "ui/uimgr"
local DY_DATA = MERequire "datamgr/dydata.lua"
local UI_DATA = MERequire "datamgr/uidata.lua"
local NW = MERequire "network/networkmgr"
local Ref
local ProjectList
--!*以下：自动生成的回调函数*--

local function on_subproject_grpproject_entproject_btndata_click(btn)
	local index = tonumber(btn.transform.parent.name:sub(11))
	local Project = ProjectList[index]
	UI_DATA.WNDSupWorkProject.projectId = Project.id
	UIMGR.create_window("UI/WNDSupWorkProject")
end

local function on_subproject_grpproject_entproject_btntask_click(btn)
	local index = tonumber(btn.transform.parent.name:sub(11))
	local Project = ProjectList[index]
	
	UI_DATA.WNDSelectStore.type = 1
	UI_DATA.WNDSelectStore.projectId = Project.id
	UI_DATA.WNDSelectStore.on_selected = function (storeId)
		-- UI_DATA.WNDSupTask.storeId = storeId
		-- UIMGR.create_window("UI/WNDSupTask")
		UI_DATA.WNDSupTaskList.storeId = storeId
		UIMGR.create_window("UI/WNDSupTaskList")
	end

	UIMGR.create_window("UI/WNDSelectStore")
end

local function on_subbtm_btnatt_click(btn)
-- ##	UIMGR.WNDStack:pop()
	UIMGR.create_window("UI/WNDSupAttendance")
end

local function on_subbtm_btnsch_click(btn)
-- ##	UIMGR.WNDStack:pop()
	UIMGR.create_window("UI/WNDSupSchedule")
end

local function on_subbtm_btnmsg_click(btn)
-- ##	UIMGR.WNDStack:pop()
	UIMGR.create_window("UI/WNDSupMsg")
end

local function on_subbtm_btnuser_click(btn)
-- ##	UIMGR.WNDStack:pop()
	UIMGR.create_window("UI/WNDSupUser")
end

local function on_ui_init()
	print("on_ui_init")
	ProjectList = DY_DATA.get_project_list()
	if ProjectList == nil then 
		print("ProjectList is nil")
		libunity.SetActive(Ref.SubProject.spNil, true)
		return 
	end
	libunity.SetActive(Ref.SubProject.spNil, #ProjectList == 0)
	
	print("ProjectList is "..#ProjectList)
	Ref.SubProject.GrpProject:dup(#ProjectList, function (i, Ent, isNew)
		local Project = ProjectList[i]
		Ent.lbText.text = Project.name
		UIMGR.get_photo(Ent.spIcon, Project.icon)
		local clr = i % 3
		libunity.SetActive(Ent.spRed, clr == 1)
		libunity.SetActive(Ent.spBlue, clr == 2)
		libunity.SetActive(Ent.spYellow, clr == 0)
	end)
end

local function init_view()
	Ref.SubProject.GrpProject.Ent.btnData.onAction = on_subproject_grpproject_entproject_btndata_click
	Ref.SubProject.GrpProject.Ent.btnTask.onAction = on_subproject_grpproject_entproject_btntask_click
	Ref.SubBtm.btnAtt.onAction = on_subbtm_btnatt_click
	Ref.SubBtm.btnSch.onAction = on_subbtm_btnsch_click
	Ref.SubBtm.btnMsg.onAction = on_subbtm_btnmsg_click
	Ref.SubBtm.btnUser.onAction = on_subbtm_btnuser_click
	UIMGR.make_group(Ref.SubProject.GrpProject, function (New, Ent)
		New.btnData.onAction = Ent.btnData.onAction
		New.btnTask.onAction = Ent.btnTask.onAction
	end)
	--!*以上：自动注册的回调函数*--
end

local function init_logic()
	NW.subscribe("WORK.SC.GETPROJECT", on_ui_init)

	if DY_DATA.ProjectList == nil or next(DY_DATA.ProjectList) == nil then
		if NW.connected() then
			local nm = NW.msg("WORK.CS.GETPROJECT")
			nm:writeU32(DY_DATA.User.id)
			NW.send(nm)
		end
		return
	end
	on_ui_init()

	-- local ProjectList = DY_DATA.ProjectList
	
	-- Ref.SubProject.GrpProject:dup(#ProjectList, function (i, Ent, isNew)
	-- 	local Project = ProjectList[i]
	-- 	Ent.lbText.text = Project.name
	-- end)
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

