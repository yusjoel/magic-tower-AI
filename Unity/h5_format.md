animates\
floors\			每层的布局
images\
sounds\
data.js			基本信息, 如用到的楼层, 英雄初始信息, 初始位置等
enemys.js		怪的信息
events.js
functions.js
icons.js		指明images目录下各个精灵图集中单个图集的名字
items.js		道具说明及功能
maps.js			定义各个id的含义

1. 读取data.js, 获取data.main.floorIds
2. 读取所有楼层? 还是指定楼层? 还是分别读取各个楼层?
3. 根据选择到floors目录下读取相应楼层数据
	main.floors.<floor_name>.map
	根据读取的地图, 到maps.js中读取各个id的定义, 由此来确定怪物/道具的范围
4. 传送点根据main.floors.<floor_name>.events 中type = "changeFloor"来决定
5. 道具属性读取data.js下的data.main.values
6. 剑,盾属性是用脚本配置的, 在items.js
7. 怪信息在enemys.js下
8. 商店属性在data.js中 (但是目前不考虑商店)
9. 英雄初始属性在data.js中, data.main.firstData.hero