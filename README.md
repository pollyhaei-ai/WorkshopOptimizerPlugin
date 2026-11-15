# WorkshopOptimizerPlugin

FFXIV Island Sanctuary workshop optimizer.

台灣版用戶端（XIVTCLauncher、yanmucorp Dalamud API 13）的移植。介面與物品名稱為繁體中文。
上游是 [belzaru17/WorkshopOptimizerPlugin](https://github.com/belzaru17/WorkshopOptimizerPlugin)，作者 HappySprite。本程式以 MIT 授權散布，著作權聲明見 `LICENSE`。

# 用法（繁體中文）

1. 前往自己的無人島。
2. 和工坊的魔法人偶對話，打開「工坊生產計畫」，或打開「海島產品需求調查」。
3. 輸入 `/wso`。
4. 選擇要排的生產日，勾選要做的海島產品。四座開拓工坊都會列入計算。
5. 把「生產計畫」分頁裡選好的產品，照樣填進遊戲內的工坊生產計畫。
6. 週期第一個生產日的預測不一定準。要在該週期第 1 到第 4 個生產日做過上面的步驟，後面的預測才比較準。

型態寫成「第2日弱」「第3日強」：數字是高峰落在第幾個生產日，弱代表供給會到「供少於求」，強代表會到「嚴重供少於求」。標了 `*` 的供需是預測。

測試用壓縮檔是 `dist/WorkshopOptimizerPlugin-0.3.2-tw.zip`。裡面的 `WorkshopOptimizerPlugin` 資料夾放到 Dalamud 的 devPlugins，在遊戲裡啟用。這個組建只給 API 13，不要裝到官方 Dalamud API 14 或 15。

# Usage

1. Go to your island
2. Talk to the Tactful Taskmaster NPC and open the workshop agenda
3. Open "Review Supply & Demand" window
4. Type /wso
5. Select the cycle you want to optimize and select the items you want to craft
6. Set those items (now in Produced tab) to the in-game workshop agenda
7. Profit!

NOTE: Predictions on the 1st day of the cycle are not 100% reliable.  You need to do the above during days 1-4 of the cycle to get accurate predictions.

This Taiwan build shows Traditional Chinese only. The steps above are the same actions as the Chinese section.

# Tabs

## Workshops
Shows possible combinations of items using all 4 workshops and expected values given Supply/Demand patterns.  Allows you to select which items you'll produce.  These items also need to be set in-game, but will be used by the plugin when calculating future cycle options and profits.

## Produced
Shows the selected items to be produced.  Selecting items here only has effect within the plugin, it doesn't change your workshop agenda.

## Next Week
Shows rare materials from the Granary that will be popular next week.

## Items
This tab shows information about all items.  Supply/Demand with an * means it's a prediction.
It also allows you to specify which items to use and when.

## Patterns
Shows predicted patterns of Supply/Demand for items for the selected cycle.  Each pattern is represented by a peak day and a weak or strong marker. Weak means supply will be Insufficient, and strong means supply will be Nonexistent.

## Combinations
Shows possible combinations of items and expected values given Supply/Demand patterns.  Allows you to select which items you'll produce.  These items also need to be set in-game, but will be used by the plugin when calculating future cycle options and profits.
