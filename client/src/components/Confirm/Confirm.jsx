import React, { Component } from 'react';
import PropTypes from 'prop-types';
import Alert from 'react-bootstrap/Alert';
import Button from 'react-bootstrap/Button';

class Confirm extends Component{

  render(){

    const { props } = this;

    if (props.visible){

      return (
          <Alert variant={props.variant}>
            <h4>{props.headerText}</h4>
            <p>
              <Button onClick={props.cancelAction} variant="danger">{props.cancelText}</Button>
              <Button onClick={props.confirmAction} variant="success">{props.confirmText}
              </Button>
            </p>
          </Alert>
        );
    }

    return (<div/>);
  }
}

Confirm.defaultProps = {
  confirmText: 'Confirm',
  cancelText: 'Cancel',
  headerText: 'Confirm action',
  variant: 'danger'
}

Confirm.propTypes = {
  visible: PropTypes.bool.isRequired,
  confirmAction: PropTypes.func.isRequired,
  cancelAction: PropTypes.func.isRequired,
  confirmText: PropTypes.string,
  cancelText: PropTypes.string,
  headerText: PropTypes.string,
  variant: PropTypes.string
};

export default Confirm;
