DROP PROCEDURE IF EXISTS sp_change_ticket_status;

DELIMITER //

CREATE PROCEDURE sp_change_ticket_status(
    IN p_ticket_id INT,
    IN p_new_status VARCHAR(20),
    IN p_comment VARCHAR(500),
    IN p_diagnosis VARCHAR(500)
)
BEGIN
    DECLARE v_previous_status VARCHAR(20);

    DECLARE EXIT HANDLER FOR SQLEXCEPTION
    BEGIN
        ROLLBACK;
        RESIGNAL;
    END;

    START TRANSACTION;

    SELECT Status
    INTO v_previous_status
    FROM Tickets
    WHERE TicketId = p_ticket_id
    FOR UPDATE;

    IF v_previous_status IS NULL THEN
        SIGNAL SQLSTATE '45000'
        SET MESSAGE_TEXT = 'Ticket not found';
    END IF;

    UPDATE Tickets
    SET
        Status = p_new_status,
        Diagnosis = COALESCE(p_diagnosis, Diagnosis),
        UpdatedAt = UTC_TIMESTAMP()
    WHERE TicketId = p_ticket_id;

    INSERT INTO TicketHistories
    (
        TicketId,
        PreviousStatus,
        NewStatus,
        Comment,
        CreatedAt
    )
    VALUES
    (
        p_ticket_id,
        v_previous_status,
        p_new_status,
        p_comment,
        UTC_TIMESTAMP()
    );

    COMMIT;
END //

DELIMITER ;