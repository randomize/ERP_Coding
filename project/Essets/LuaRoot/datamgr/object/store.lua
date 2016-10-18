--
-- @file    datamgr/object/store.lua
-- @authors zhaole
-- @date    2016-8-14 16:08:17
-- @desc    项目类
-- 

local string, table
    = string, table
local libunity = require "libunity.cs"
local libugui = require "libugui.cs"
local libasset = require "libasset.cs"
local libbattle = require "libbattle.cs"
local DY_DATA = MERequire "datamgr/dydata"
local CFGLIB = _G.CFG

local OBJDEF = {}
OBJDEF.__index = OBJDEF
OBJDEF.__tostring = function (self)
    return string.format("[门店#%d]", self.id)
end

function OBJDEF.new(id)
    local self = { 
        id = id,
    }
    return setmetatable(self, OBJDEF)
end

return OBJDEF
