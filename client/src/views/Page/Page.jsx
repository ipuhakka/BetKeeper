import React, { Component } from 'react';
import _ from 'lodash';
import { withRouter } from 'react-router-dom';
import * as PageActions from '../../actions/pageActions';
import * as PageUtils from '../../js/pageUtils'; 
import Header from '../../components/Header/Header';
import Menu from '../../components/Menu/Menu';
import Confirm from '../../components/Confirm/Confirm';
import Modal from '../../components/Page/Modal';
import PageContent from '../../components/Page/PageContent';
import Info from '../../components/Info/Info.jsx';
import './Page.css';

class Page extends Component
{
    constructor(props)
    {
		super(props);

		this.state = {
            actionModalOpen: false,
            actionModalProps: {},
            confirm: {
                show: false,
                action: '',
                actionDataKeys: [],
                header: ''
            }
        };
        
        this.clickModalAction = this.clickModalAction.bind(this);
        this.closeConfirm = this.closeConfirm.bind(this);
        this.navigateTo = this.navigateTo.bind(this);
        this.clickPageAction = this.clickPageAction.bind(this);
        this.onDataChange = this.onDataChange.bind(this);
        this.executePageAction = this.executePageAction.bind(this);
        this.executePageActionFromConfirm = this.executePageActionFromConfirm.bind(this);
        this.handleDropdownServerUpdate = this.handleDropdownServerUpdate.bind(this);
    }

    /**
     * Handles data change. Data is stored as a nested object. Containers store its child data in inner object.
     * If container actually stores data as array, its data needs to be converted to array when getting action parameters.
     * @param {string} path Variable key path. 
     * @param {*} newValue 
     */
    onDataChange(path, newValue)
    {
        PageActions.onDataChange(this.props.pageKey, path, newValue);
    }

    /**
     * Handles a dropdown server update.
     * @param {string} dropdownKey 
     * @param {*} value 
     * @param {Array} componentsToUpdate 
     */
    handleDropdownServerUpdate(dropdownKey, value, componentsToUpdate)
    {
        const { props } = this;
        
        const components = _.map(componentsToUpdate, componentKey =>
            {
                return PageUtils.findComponentFromPage(
                    { components: props.components },
                    componentKey
                );
            })

        PageActions.handleServerDropdownUpdate(dropdownKey, value, components);
    }

    /**
     * Executes a page action after confirm.
     */
    executePageActionFromConfirm()
    {
        const { onSuccessNavigateTo } = this.state;
        const {action, actionDataKeys, componentsToInclude } = this.state.confirm;

        this.executePageAction(action, actionDataKeys, onSuccessNavigateTo, componentsToInclude)
    }

    /**
     * Executes a page action.
     * @param {string} action 
     * @param {Array} actionDataKeys 
     * @param {string} onSuccessNavigateTo Navigate to this address on success
     */
    executePageAction(action, actionDataKeys, onSuccessNavigateTo, componentsToInclude)
    {
        const { props } = this;

        const data = props.data;

        const parameters = PageUtils.getActionData(data, actionDataKeys, props.components, componentsToInclude);

        const pageKey = PageUtils.getActivePageName();

        PageActions.callAction(pageKey, action, parameters, _.isNil(onSuccessNavigateTo)
            ? null
            : () => 
            {
                this.navigateTo(onSuccessNavigateTo);
            }
        );
    }

    /**
     * Handles page action button click. Either opens a confirm dialog
     * or calls action directly.
     * @param {string} action action name
     * @param {Array.<string>} actionDataKeys Data keys used in action.
     * @param {bool} requireConfirm Will a confirm dialog be displayed before completing action 
     * @param {string} confirmHeader Header for confirm dialog
     * @param {string} confirmStyle Confirm dialog style
     * @param {string} onSuccessNavigateTo If given, redirect to this address after successful action 
     */
    clickPageAction(
        action, 
        actionDataKeys, 
        requireConfirm, 
        componentsToInclude,
        confirmHeader, 
        confirmStyle, 
        onSuccessNavigateTo = null)
    {
        if (requireConfirm)
        {
            this.setState({
                confirm: {
                    show: true,
                    action,
                    actionDataKeys,
                    componentsToInclude,
                    header: confirmHeader,
                    variant: confirmStyle
                },
                onSuccessNavigateTo
            });
        }
        else 
        {
            this.executePageAction(action, actionDataKeys, onSuccessNavigateTo, componentsToInclude);
        }
    }

    /**
     * Handle modal action click
     * @param {string} action 
     * @param {Array.<object>} actionFields Fiels to be shown in action modal.
     * @param {title} title 
     * @param {bool} requireConfirm 
     * @param {string} confirmStyle 
     * @param {string} absoluteDataPath
     */
    clickModalAction(action, components, title, requireConfirm, confirmStyle, absoluteDataPath)
    {
        this.setState({
            actionModalOpen: true,
            actionModalProps: {
                action,
                title,
                requireConfirm,
                confirmVariant: confirmStyle,
                components,
                absoluteDataPath
            }
        });
    }

    /**
     * Navigates to a given address
     * @param {string} address 
     */
    navigateTo(address)
    {
        this.props.history.push(address);
    }

    /**
     * Returns function for button click event handling.
     * @param {object} button 
     */
    getButtonClick(button)
    {
        if (button.buttonType === 'ModalAction')
        {
            return this.clickModalAction;
        }

        if (button.buttonType === 'PageAction')
        {
            return this.clickPageAction;
        }

        if (button.buttonType === 'Navigation')
        {
            return this.navigateTo;
        }

        return null;
    }

    /** 
     * Closes confirm.
     */
    closeConfirm()
    {
        this.setState({
            confirm: {
                show: false,
                action: '',
                actionDataKeys: [],
                header: ''
            }
        });
    }
    
    render()
    {
        const { state, props } = this;

        const { pageKey } = props;

        return <div className="content">
            <Modal 
                onClose={() => 
                {
                    this.setState({ actionModalOpen: false });
                }} 
                page={pageKey || ''}
                show={state.actionModalOpen} 
                {...state.actionModalProps}
                data={props.data}/>
            <Header title={"Logged in as " + window.sessionStorage.getItem('loggedUser')}></Header>
			<Menu disableValue={pageKey}></Menu>
            <Confirm 
                visible={state.confirm.show}
                headerText={state.confirm.header}
                cancelAction={this.closeConfirm}
                confirmAction={() => 
                    {
                        this.executePageActionFromConfirm();
                        this.closeConfirm();
                    }}
                variant={state.confirm.variant}/>
            <Info alertState={state.alertState} alertText={state.alertText} dismiss={this.dismissAlert} />
            <PageContent 
                components={props.components}
                className='page-content'
                getButtonClick={this.getButtonClick.bind(this)}
                onTableRowClick={this.navigateTo.bind(this)}
                onFieldValueChange={this.onDataChange.bind(this)}
                onHandleDropdownServerUpdate={this.handleDropdownServerUpdate}
                data={props.data}/>
        </div>;
    }
};

export default withRouter(Page);