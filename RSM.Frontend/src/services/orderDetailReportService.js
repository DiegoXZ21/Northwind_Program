import jsPDF from "jspdf"
import autoTable from "jspdf-autotable"

const statusMap = {
  0: "Pending",
  1: "Processing",
  2: "Shipped",
  3: "Completed",
  4: "Cancelled"
}

export function generateOrderDetailPDF(order) {
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
    doc.text("ORDER DETAIL REPORT", 14, 18)

    // FOOTER
    doc.setFillColor(31, 78, 120)
    doc.rect(0, pageHeight - 12, 210, 12, "F")

    doc.setFontSize(9)
    doc.setTextColor(255, 255, 255)
    doc.text(
      "Northwind Traders System © 2026",
      14,
      pageHeight - 4
    )

    doc.text(
      `Page ${data.pageNumber} of ${pageCount}`,
      180,
      pageHeight - 4
    )
  }

  // INFO GENERAL
  doc.setTextColor(0, 0, 0)
  doc.setFontSize(10)

  doc.text(`Order ID: ${order.orderId}`, 14, 30)
  doc.text(`Customer: ${order.customerName}`, 14, 36)
  doc.text(`Date: ${new Date(order.orderDate).toLocaleDateString()}`, 14, 42)
  doc.text(`Status: ${statusMap[order.status]}`, 14, 48)

  // SHIPPING
  doc.text("Shipping:", 14, 58)
  doc.text(`${order.shippingAddress || ""}`, 14, 64)
  doc.text(`${order.city || ""}, ${order.country || ""}`, 14, 70)

  // TOTAL
  const total = order.products.reduce(
    (sum, p) => sum + (p.unitPrice * p.quantity * (1 - p.discount)),
    0
  )

  doc.setFont(undefined, "bold")
  doc.text(`Total: $${total.toFixed(2)}`, 150, 48)
  doc.setFont(undefined, "normal")

  // TABLA PRODUCTOS
  autoTable(doc, {
    startY: 80,
    head: [["Product", "Price", "Qty", "Discount", "Subtotal"]],
    body: order.products.map(p => [
      p.productName,
      Number(p.unitPrice).toFixed(2),
      p.quantity,
      (p.discount * 100).toFixed(0) + "%",
      (p.unitPrice * p.quantity * (1 - p.discount)).toFixed(2)
    ]),

    styles: {
      fontSize: 9
    },

    headStyles: {
      fillColor: [47, 85, 151],
      textColor: 255
    },

    alternateRowStyles: {
      fillColor: [245, 247, 250]
    },

    didDrawPage: drawHeaderFooter
  })

  doc.save(`order-${order.orderId}.pdf`)
}