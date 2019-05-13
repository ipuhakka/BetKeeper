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
            <Alert.Heading>{props.headerText}</Alert.Heading>
            <div className="d-flex justify-content-center">
              <Button onClick={props.cancelAction} variant="outline-danger">
                {props.cancelText}
              </Button>
              <Button onClick={props.confirmAction} variant="outline-success">
              {props.confirmText}
              </Button>
          </div>
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
  confirmText: PropTypes.string.isRequired,
  cancelText: PropTypes.string.isRequired,
  headerText: PropTypes.string,
  variant: PropTypes.string
};

export default Confirm;
