import React, { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { Card, Spinner, Table, Badge, Button, Row, Col } from 'react-bootstrap';
<<<<<<< HEAD
import { Order, orderApi } from '../api/orderApi';

const OrderDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [order, setOrder] = useState<Order | null>(null);
=======

interface OrderDetail {
  id: number;
  customerName: string;
  customerEmail: string;
  customerAddress: string;
  status: string;
  totalAmount: number;
  createdAt: string;
  items: Array<{
    productId: number;
    productName: string;
    quantity: number;
    unitPrice: number;
  }>;
  inventoryRecord?: {
    id: number;
    status: string;
    checkedAt: string;
  };
  paymentRecord?: {
    id: number;
    status: string;
    amount: number;
    processedAt: string;
  };
  shipmentRecord?: {
    id: number;
    trackingNumber: string;
    carrier: string;
    shippedAt: string;
  };
}

const OrderDetails: React.FC = () => {
  const { id } = useParams<{ id: string }>();
  const [order, setOrder] = useState<OrderDetail | null>(null);
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    fetchOrderDetails();
  }, [id]);

  const fetchOrderDetails = async () => {
    try {
<<<<<<< HEAD
      const data = await orderApi.getOrderById(Number(id));
      setOrder(data);
    } catch {
=======
      const response = await fetch(`http://localhost:5000/api/orders/${id}`);
      if (!response.ok) throw new Error('Failed to fetch order details');
      const data = await response.json();
      setOrder(data);
    } catch (err) {
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
      setError('Unable to connect to Order API');
    } finally {
      setLoading(false);
    }
  };

<<<<<<< HEAD
  const getStatusBadge = (status?: string) => {
    const statusColors: Record<string, string> = {
      Submitted: 'secondary',
      PaymentPending: 'primary',
      ShippingPending: 'info',
      ShippingCreated: 'info',
      Completed: 'success',
      Failed: 'danger'
    };
    const label = status || 'Unknown';
    return <Badge bg={statusColors[label] || 'secondary'}>{label}</Badge>;
=======
  const getStatusBadge = (status: string) => {
    const statusColors: Record<string, string> = {
      Pending: 'secondary',
      InventoryChecked: 'info',
      PaymentApproved: 'primary',
      Shipped: 'success',
      Completed: 'success',
      Cancelled: 'danger',
      Failed: 'danger'
    };
    return <Badge bg={statusColors[status] || 'secondary'}>{status}</Badge>;
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
  };

  if (loading) {
    return (
      <div className="text-center mt-5">
        <Spinner animation="border" role="status">
          <span className="visually-hidden">Loading...</span>
        </Spinner>
      </div>
    );
  }

  if (error || !order) {
    return (
      <div className="alert alert-warning" role="alert">
        {error || 'Order not found'}
      </div>
    );
  }

  return (
    <div>
      <div className="d-flex justify-content-between align-items-center mb-4">
<<<<<<< HEAD
        <h2>Order #{order.orderId}</h2>
=======
        <h2>Order #{order.id}</h2>
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
        <Link to="/orders">
          <Button variant="secondary">Back to Orders</Button>
        </Link>
      </div>

      <Card className="mb-4">
        <Card.Header>Order Status</Card.Header>
        <Card.Body>
          <div className="d-flex justify-content-between align-items-center">
            <div>
<<<<<<< HEAD
              <h4>{getStatusBadge(order.orderStatus)}</h4>
=======
              <h4>{getStatusBadge(order.status)}</h4>
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
              <p className="text-muted mb-0">Created: {new Date(order.createdAt).toLocaleString()}</p>
            </div>
            <div className="text-end">
              <h3>Total: ${order.totalAmount.toFixed(2)}</h3>
            </div>
          </div>
        </Card.Body>
      </Card>

      <Row className="mb-4">
        <Col md={6}>
          <Card>
            <Card.Header>Customer Information</Card.Header>
            <Card.Body>
<<<<<<< HEAD
              <p><strong>Name:</strong> {order.name}</p>
              <p><strong>Customer ID:</strong> {order.customerId || 'N/A'}</p>
              <p><strong>Address:</strong> {[order.line1, order.line2, order.line3, `${order.city || ''}${order.state ? `, ${order.state}` : ''} ${order.zip || ''}`, order.country].filter(Boolean).join(', ')}</p>
=======
              <p><strong>Name:</strong> {order.customerName}</p>
              <p><strong>Email:</strong> {order.customerEmail}</p>
              <p><strong>Address:</strong> {order.customerAddress}</p>
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
            </Card.Body>
          </Card>
        </Col>
        <Col md={6}>
          <Card>
            <Card.Header>Order Items</Card.Header>
            <Card.Body className="p-0">
              <Table className="mb-0" size="sm">
                <thead>
                  <tr>
                    <th>Product</th>
                    <th>Qty</th>
                    <th>Price</th>
                  </tr>
                </thead>
                <tbody>
<<<<<<< HEAD
                  {(order.items || []).map((item, idx) => (
=======
                  {order.items.map((item, idx) => (
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
                    <tr key={idx}>
                      <td>{item.productName}</td>
                      <td>{item.quantity}</td>
                      <td>${item.unitPrice.toFixed(2)}</td>
                    </tr>
                  ))}
                </tbody>
              </Table>
            </Card.Body>
          </Card>
        </Col>
      </Row>

      <Row>
        <Col md={4}>
          <Card>
            <Card.Header>Inventory Check</Card.Header>
            <Card.Body>
              {order.inventoryRecord ? (
                <>
                  <p><strong>Status:</strong> {getStatusBadge(order.inventoryRecord.status)}</p>
                  <p><strong>Checked:</strong> {new Date(order.inventoryRecord.checkedAt).toLocaleString()}</p>
                </>
              ) : (
                <p className="text-muted">No inventory record</p>
              )}
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
          <Card>
            <Card.Header>Payment</Card.Header>
            <Card.Body>
              {order.paymentRecord ? (
                <>
                  <p><strong>Status:</strong> {getStatusBadge(order.paymentRecord.status)}</p>
                  <p><strong>Amount:</strong> ${order.paymentRecord.amount.toFixed(2)}</p>
                  <p><strong>Processed:</strong> {new Date(order.paymentRecord.processedAt).toLocaleString()}</p>
                </>
              ) : (
                <p className="text-muted">No payment record</p>
              )}
            </Card.Body>
          </Card>
        </Col>
        <Col md={4}>
          <Card>
            <Card.Header>Shipping</Card.Header>
            <Card.Body>
              {order.shipmentRecord ? (
                <>
                  <p><strong>Tracking:</strong> {order.shipmentRecord.trackingNumber}</p>
                  <p><strong>Carrier:</strong> {order.shipmentRecord.carrier}</p>
<<<<<<< HEAD
                  <p><strong>Status:</strong> {getStatusBadge(order.shipmentRecord.status)}</p>
                  <p><strong>Dispatch:</strong> {order.shipmentRecord.estimatedDispatchDate ? new Date(order.shipmentRecord.estimatedDispatchDate).toLocaleString() : 'Pending'}</p>
=======
                  <p><strong>Shipped:</strong> {new Date(order.shipmentRecord.shippedAt).toLocaleString()}</p>
>>>>>>> d53dbaa649fb2b7845b5dc91ce1f794377a85d00
                </>
              ) : (
                <p className="text-muted">No shipment record</p>
              )}
            </Card.Body>
          </Card>
        </Col>
      </Row>
    </div>
  );
};

export default OrderDetails;
