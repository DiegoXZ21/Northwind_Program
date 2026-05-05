import ExcelJS from "exceljs"

export async function generateOrdersExcel(orders, filters = {}) {

    const workbook = new ExcelJS.Workbook()
    const sheet = workbook.addWorksheet("Orders")

    sheet.mergeCells("A1:F1")
    const title = sheet.getCell("A1")
    title.value = "NORTHWIND TRADERS SYSTEM"
    title.font = { size: 16, bold: true, color: { argb: "FFFFFFFF" } }
    title.alignment = { horizontal: "center" }
    title.fill = {
        type: "pattern",
        pattern: "solid",
        fgColor: { argb: "FF1F4E78" }
    }

    sheet.mergeCells("A2:F2")
    const subtitle = sheet.getCell("A2")
    subtitle.value = "MASSIVE ORDERS REPORT"
    subtitle.font = { size: 12, bold: true }

    sheet.mergeCells("A3:F3")
    sheet.getCell("A3").value = `Generated: ${new Date().toLocaleDateString()}`

    sheet.mergeCells("A4:F4")
    sheet.getCell("A4").value =
        `Filters -> Year: ${filters.year || "ALL"} | Month: ${filters.month || "ALL"} | Country: ${filters.country || "ALL"}`

    const headerRow = sheet.addRow([
        "ID", "Customer", "Date", "Country", "Status", "Total"
    ])

    headerRow.eachCell(cell => {
        cell.font = { bold: true, color: { argb: "FFFFFFFF" } }
        cell.alignment = { horizontal: "center" }
        cell.fill = {
            type: "pattern",
            pattern: "solid",
            fgColor: { argb: "FF2F5597" }
        }
        cell.border = {
            top: { style: "thin" },
            left: { style: "thin" },
            bottom: { style: "thin" },
            right: { style: "thin" }
        }
    })

    orders.forEach(o => {
        const row = sheet.addRow([
            o.orderId,
            o.customerName,
            o.orderDate ? new Date(o.orderDate).toLocaleDateString() : "",
            o.country || "",
            o.status,
            Number(o.totalAmount ?? 0)
        ])

        row.eachCell(cell => {
            cell.border = {
                top: { style: "thin" },
                left: { style: "thin" },
                bottom: { style: "thin" },
                right: { style: "thin" }
            }
            cell.alignment = { horizontal: "center" }
        })
    })

    const total = orders.reduce((sum, o) => sum + (o.totalAmount ?? 0), 0)

    const totalRow = sheet.addRow(["", "", "", "", "TOTAL", total.toFixed(2)])

    totalRow.eachCell(cell => {
        cell.font = { bold: true }
        cell.fill = {
            type: "pattern",
            pattern: "solid",
            fgColor: { argb: "FFFFD966" }
        }
        cell.border = {
            top: { style: "thin" },
            left: { style: "thin" },
            bottom: { style: "thin" },
            right: { style: "thin" }
        }
    })

    const footerRowIndex = sheet.lastRow.number + 2
    sheet.mergeCells(`A${footerRowIndex}:F${footerRowIndex}`)
    const footer = sheet.getCell(`A${footerRowIndex}`)
    footer.value = "Northwind Traders System © 2026 - Confidential Business Report"
    footer.alignment = { horizontal: "center" }
    footer.font = { italic: true, size: 10 }

    sheet.columns = [
        { width: 10 },
        { width: 25 },
        { width: 18 },
        { width: 15 },
        { width: 15 },
        { width: 15 }
    ]

    const buffer = await workbook.xlsx.writeBuffer()

    const blob = new Blob([buffer], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    })

    const url = window.URL.createObjectURL(blob)

    const a = document.createElement("a")
    a.href = url
    a.download = "massive-orders-report.xlsx"
    a.click()

    window.URL.revokeObjectURL(url)
}