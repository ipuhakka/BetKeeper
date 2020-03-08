import React, { Component } from 'react';
import PropTypes from 'prop-types';
import RBModal from 'react-bootstrap/Modal';
import Button from 'react-bootstrap/Button';
import * as pageActions from '../../actions/pageActions';
import Confirm from '../Confirm/Confirm';
import PageContent from '../Page/PageContent';

/** Action modal. */
class Modal extends Component 
{
    constructor(props)
    {
      super(props);

      this.state = {
        actionResponseValues: {},
        hasInvalidInputs: false,
        confirm: {
          show: false,
          action: '',
          actionDataKeys: []
        }
      }

      this.onChangeInputValue = this.onChangeInputValue.bind(this);
    }

    onChangeInputValue(key, newValue)
    {
      const actionResponse = {...this.state.actionResponseValues};

      actionResponse[key] = newValue;

      this.setState({
        actionResponseValues: actionResponse,
        hasInvalidInputs: false
      });
    }

    render()
    {
        const { props, state } = this;
        
        return <RBModal show={props.show} onHide={props.onClose}>
          <RBModal.Header closeButton>
            <RBModal.Title>{props.title}</RBModal.Title>
          </RBModal.Header>
        
          <RBModal.Body>
            <div>
            <Confirm 
                visible={state.confirm.show}
                headerText={props.title}
                cancelAction={() => 
                {
                    this.setState({
                        confirm: {
                            show: false,
                            action: '',
                            actionDataKeys: []
                        }
                    });
                }}
                confirmAction={() => 
                {
                  pageActions.callAction(props.page, props.action, state.actionResponseValues, props.onClose)
                }}
                variant={props.confirmVariant}/>
              <PageContent 
                onFieldValueChange={this.onChangeInputValue}
                components={props.components}
                getButtonClick={() => { throw new Error('No button click')}}
                className='modal-content'
                />
            </div>
          </RBModal.Body>
        
          <RBModal.Footer>
            <Button 
              variant="outline-secondary" 
              onClick={props.onClose}>Close</Button>
            <Button 
              variant="outline-primary"
              disabled={state.hasInvalidInputs}
              onClick={() => 
              {
                if (props.requireConfirm)
                {
                  this.setState({
                    confirm: {
                      ...this.state.confirm,
                      show: true
                    }
                  })
                }
                else 
                {
                  pageActions.callAction(props.page, props.action, state.actionResponseValues, props.onClose);
                }
              }}>Ok</Button>
          </RBModal.Footer>
        </RBModal>;
    }
};

Modal.defaultProps = {
  action: '',
  components: [],
  title: '',
  requireConfirm: false
};

Modal.propTypes = {
  onClose: PropTypes.func.isRequired,
  show: PropTypes.bool.isRequired,
  page: PropTypes.string.isRequired,
  action: PropTypes.string.isRequired,
  title: PropTypes.string.isRequired,
  requireConfirm: PropTypes.bool.isRequired,
  confirmVariant: PropTypes.string,
  components: PropTypes.array.isRequired
};

export default Modal;