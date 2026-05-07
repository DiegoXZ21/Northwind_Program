import ExcelJS from "exceljs"


const statusMap = {
  0: "Pending",
  1: "Processing",
  2: "Shipped",
  3: "Completed",
  4: "Cancelled"
}


export async function generateOrderDetailExcel(order) {

    const workbook = new ExcelJS.Workbook()
    const sheet = workbook.addWorksheet("Order Detail")

    sheet.mergeCells("A1:G1")
    const title = sheet.getCell("A1")
    title.value = "NORTHWIND TRADERS SYSTEM"
    title.font = { size: 16, bold: true, color: { argb: "FFFFFFFF" } }
    title.alignment = { horizontal: "center" }
    title.fill = {
        type: "pattern",
        pattern: "solid",
        fgColor: { argb: "FF1F4E78" }
    }

    sheet.mergeCells("A2:G2")
    const subtitle = sheet.getCell("A2")
    subtitle.value = "ORDER DETAIL REPORT"
    subtitle.font = { size: 12, bold: true }

    sheet.mergeCells("A3:G3")
    sheet.getCell("A3").value = `Generated: ${new Date().toLocaleDateString()}`

    sheet.mergeCells("A4:G4")
    sheet.getCell("A4").value =
        `Order ID: ${order.orderId} | Customer: ${order.customerName || "N/A"} | Status: ${statusMap[order.status]} | Freight: $${(order.freight ?? 0).toFixed(2)}`

    const headerRow = sheet.addRow([
        "Product",
        "Qty/Unit",
        "Price",
        "Quantity",
        "Discount",
        "Subtotal",
        "Stock"
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

    order.products.forEach(p => {
        const subtotal = p.unitPrice * p.quantity * (1 - p.discount)

        const row = sheet.addRow([
            p.productName,
            p.quantityPerUnit,
            Number(p.unitPrice),
            p.quantity,
            (p.discount * 100).toFixed(2) + "%",
            subtotal.toFixed(2),
            p.unitsInStock ?? 0
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

    const total = order.products.reduce((sum, p) =>
        sum + (p.unitPrice * p.quantity * (1 - p.discount)), 0
    )

    const totalRow = sheet.addRow(["", "", "", "", "", "TOTAL", total.toFixed(2)])

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
    sheet.mergeCells(`A${footerRowIndex}:G${footerRowIndex}`)
    const footer = sheet.getCell(`A${footerRowIndex}`)
    footer.value = "Northwind Traders System © 2026 - Confidential Business Report"
    footer.alignment = { horizontal: "center" }
    footer.font = { italic: true, size: 10 }

    sheet.columns = [
        { width: 25 },
        { width: 20 },
        { width: 15 },
        { width: 12 },
        { width: 15 },
        { width: 15 },
        { width: 12 }
    ]

    const buffer = await workbook.xlsx.writeBuffer()

    const blob = new Blob([buffer], {
        type: "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    })

    const url = window.URL.createObjectURL(blob)

    const a = document.createElement("a")
    a.href = url
    a.download = `order-detail-${order.orderId}.xlsx`
    a.click()

    window.URL.revokeObjectURL(url)
}