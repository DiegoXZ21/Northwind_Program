import jsPDF from "jspdf"
import autoTable from "jspdf-autotable"

const statusMap = {
    0: "Pending",
    1: "Processed",
    2: "Shipped",
    3: "Completed",
    4: "Cancelled"
}

export function generateOrdersPDF(orders, filters = {}) {
    const doc = new jsPDF()

    const drawHeaderFooter = (data) => {
        const pageCount = doc.internal.getNumberOfPages()
        const pageHeight = doc.internal.pageSize.height

        // HEADER
        doc.setFillColor(31, 78, 120)
        doc.rect(0, 0, 210, 22, "F")

        doc.setFontSize(14)
        doc.setTextColor(255, 255, 255)
        doc.text("NORTHWIND TRADERS", 14, 12)

        doc.setFontSize(10)
        doc.text("MASSIVE ORDERS REPORT", 14, 18)

        // FOOTER
        doc.setFillColor(31, 78, 120)
        doc.rect(0, pageHeight - 12, 210, 12, "F")

        doc.setFontSize(9)
        doc.setTextColor(255, 255, 255)
        doc.text(
            "Northwind Traders System © 2026 - Confidential Report",
            14,
            pageHeight - 4
        )

        doc.text(
            `Page ${data.pageNumber} of ${pageCount}`,
            180,
            pageHeight - 4
        )
    }

    doc.setFontSize(10)
    doc.setTextColor(0, 0, 0)
    doc.text(`Generated: ${new Date().toLocaleDateString()}`, 14, 30)

    doc.text(
        `Filters -> Year: ${filters.year || "ALL"} | Month: ${filters.month || "ALL"} | Country: ${filters.country || "ALL"}`,
        14,
        36
    )

    const total = orders.reduce((sum, o) => sum + (o.totalAmount ?? 0), 0)

    doc.setFontSize(11)
    doc.setFont(undefined, "bold")
    doc.text(`Total Sales: $${total.toFixed(2)}`, 150, 36)
    doc.setFont(undefined, "normal")

    autoTable(doc, {
        startY: 45,
        margin: { top: 30 },
        head: [["ID", "Customer", "Date", "Country", "Status", "Total"]],
        body: orders.map(o => [
            o.orderId,
            o.customerName,
            o.orderDate ? new Date(o.orderDate).toLocaleDateString() : "",
            o.country || "N/A",
            statusMap[o.status] || o.status,
            Number(o.totalAmount ?? 0).toFixed(2)
        ]),

        styles: {
            fontSize: 9,
            cellPadding: 3,
            lineColor: [200, 200, 200],
            lineWidth: 0.2
        },

        headStyles: {
            fillColor: [47, 85, 151],
            textColor: 255,
            halign: "center"
        },

        columnStyles: {
            0: { halign: "center" },
            2: { halign: "center" },
            4: { halign: "center" },
            5: { halign: "right" }
        },

        alternateRowStyles: {
            fillColor: [245, 247, 250]
        },

        didDrawPage: drawHeaderFooter
    })

    doc.save("massive-orders-report.pdf")
}