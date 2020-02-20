import React, { Component } from 'react';
import PropTypes from 'prop-types';
import RBModal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';

class Modal extends Component 
{

    render()
    {
        const { props } = this;

        return <RBModal show={props.show} onHide={props.onClose}>
        <RBModal.Header closeButton>
          <RBModal.Title>Modal title</RBModal.Title>
        </RBModal.Header>
      
        <RBModal.Body>
          <p>Modal body text goes here.</p>
        </RBModal.Body>
      
        <RBModal.Footer>
          <Button variant="outline-secondary" onClick={props.onClose}>Close</Button>
          <Button variant="primary">Save changes</Button>
        </RBModal.Footer>
      </RBModal>;
    }
};

export default Modal;