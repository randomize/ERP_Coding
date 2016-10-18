--
-- @file    ui/login/lc_wndbindphone.lua
-- @authors ckxz
-- @date    2016-07-04 16:40:45
-- @desc    WNDBindPhone
--

local ipairs, pairs
    = ipairs, pairs
local libugui = require "libugui.cs"
local libunity = require "libunity.cs"
local UIMGR = MERequire "ui/uimgr"
local UI_DATA = MERequire "datamgr/uidata.lua"
local LOGIN = MERequire "libmgr/login.lua"
local Ref

local type
local function on_get_verifyed(Ret)
	if Ret.ret == 1 then
		UIMGR.create_window("UI/WNDVerifyPhone")
	end
end
--!*以下：自动生成的回调函数*--

local function on_submain_btnenter_click(btn)
	local phone = Ref.SubMain.SubPhone.inpPhone.text
	local UserInfo = UI_DATA.WNDRegist.UserInfo
	UserInfo.phone = phone
	
	LOGIN.try_get_verify(phone, type, on_get_verifyed)
end

local function on_subtop_btnback_click(btn)
	UIMGR.close_window(Ref.root)
end

local function init_view()
	Ref.SubMain.btnEnter.onAction = on_submain_btnenter_click
	Ref.SubTop.btnBack.onAction = on_subtop_btnback_click
	--!*以上：自动注册的回调函数*--
end

local function init_logic()
	type = UI_DATA.WNDBindPhone.type
	UI_DATA.WNDBindPhone.type = nil
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

local P = {
	start = start,
	update_view = update_view,
}
return P

