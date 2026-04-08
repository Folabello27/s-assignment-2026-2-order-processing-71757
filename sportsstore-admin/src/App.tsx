import React from 'react';
import { BrowserRouter as Router, Routes, Route, NavLink } from 'react-router-dom';
import { Navbar, Nav, Container } from 'react-bootstrap';
import Dashboard from './pages/Dashboard';
import Orders from './pages/Orders';
import OrderDetails from './pages/OrderDetails';
import FailedOrders from './pages/FailedOrders';

function App() {
  return (
    <Router>
      <div className="App">
        <Navbar bg="dark" variant="dark" expand="lg" className="mb-4">
          <Container>
            <Navbar.Brand href="/">
              <i className="fas fa-cog me-2"></i>
              SportsStore Admin
            </Navbar.Brand>
            <Navbar.Toggle aria-controls="basic-navbar-nav" />
            <Navbar.Collapse id="basic-navbar-nav">
              <Nav className="me-auto">
                <Nav.Link as={NavLink} to="/" end>
                  Dashboard
                </Nav.Link>
                <Nav.Link as={NavLink} to="/orders">
                  Orders
                </Nav.Link>
                <Nav.Link as={NavLink} to="/failed-orders">
                  Failed Orders
                </Nav.Link>
              </Nav>
            </Navbar.Collapse>
          </Container>
        </Navbar>

        <Container>
          <Routes>
            <Route path="/" element={<Dashboard />} />
            <Route path="/orders" element={<Orders />} />
            <Route path="/orders/:id" element={<OrderDetails />} />
            <Route path="/failed-orders" element={<FailedOrders />} />
          </Routes>
        </Container>
      </div>
    </Router>
  );
}

export default App;
